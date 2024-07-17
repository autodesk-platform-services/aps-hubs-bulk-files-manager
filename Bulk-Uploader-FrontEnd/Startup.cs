using Ac.Net.Authentication;
using Ac.Net.Authentication.Models;
using ApsSettings.Data;
using ApsSettings.Data.DataUtils;
using ApsSettings.Data.Models;
using Bulk_Uploader_Electron.ClientApp.src.Utilities;
using Bulk_Uploader_Electron.Jobs;
using Bulk_Uploader_Electron.Utils;
using Hangfire;
using Hangfire.MemoryStorage;
using mass_upload_via_s3_csharp;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PluginBase;
using System.Reflection;

namespace Bulk_Uploader_Electron
{
    public class Startup
    {
        private readonly string _databaseName = "Bulk-Uploader";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private static IServiceProvider _serviceProvider;

        private static void OnTokenRefresh(ITokenManager manager, TokenData token)
        {
            var context = _serviceProvider.GetService<DataContext>();

            if (context == null) { return; }
            var refreshToken = SettingsUtiltities.GetSetting(context, "RefreshToken");

            if (token == null)
            {
                refreshToken.Value = "";
            }
            else
            {
                refreshToken.Value = token.refresh_token ?? "";
            }
            AppSettings.Instance.RefreshToken = refreshToken.Value;
            context.SaveChanges();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public async void ConfigureServices(IServiceCollection services)
        {
            List<Assembly> assemblies = new List<Assembly>()
            {
                typeof(MigrationPlugin.Plugin.MigrationPluginObj).Assembly,
                typeof(TestPlugin.Plugin.TestPluginObj).Assembly
            };

            services.AddControllersWithViews();
            services.AddMvc(opt => { })
                .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddSpaStaticFiles(cfg =>
            {
                cfg.RootPath = "ClientApp/dist";
            });

            //create db context
            services.AddDbContext<DataContext>(options => options.UseSqlite($"Data Source={_databaseName}.db"));

            _serviceProvider = services!.BuildServiceProvider();
            var settingsContext = _serviceProvider.GetService<DataContext>();
            var migrations = settingsContext!.Database.GetPendingMigrations();
            if (migrations != null)
            {
                await settingsContext.Database.MigrateAsync();
            }

            // register plugins
            foreach (var asm in assemblies)
            {
                PluginManager.Instance.RegisterAssembly(asm);
                services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(asm));
                BatchManager.Instance.RegisterAssembly(asm);
            }

            foreach (var plugin in PluginManager.Instance.Plugins)
            {
                plugin.ConfigureServices(services);
            }

            //check if db has existing id + secret rows in db, if not, create empty placeholders
            AppSettings.BuildConfig();
            var clientId = SettingsUtiltities.GetSetting(settingsContext, "ClientId");
            var clientSecret = SettingsUtiltities.GetSetting(settingsContext, "ClientSecret");
            var authCallback = SettingsUtiltities.GetSetting(settingsContext, "AuthCallback");
            var refreshToken = SettingsUtiltities.GetSetting(settingsContext, "RefreshToken");
            var hubId = SettingsUtiltities.GetSetting(settingsContext, "HubId");
            var projectId = SettingsUtiltities.GetSetting(settingsContext, "ProjectId");
            var projectOutputFolder = SettingsUtiltities.GetSetting(settingsContext, "ProjectOutputFolder");

            AppSettings.Instance.ClientId = clientId.Value;
            AppSettings.Instance.ClientSecret = clientSecret.Value;
            AppSettings.Instance.RefreshToken = refreshToken.Value;

            // services.AddHangfire(configuration => configuration.UseSQLiteStorage($"{_databaseName}.db"));
            services.AddHangfire(config => config.UseMemoryStorage());

            services.AddHangfireServer(options =>
            {
                options.Queues = new[] { "process-file" };
                int workCount = 1;
                Int32.TryParse(AppSettings.Instance.FileWorkerCount, out workCount);
                options.WorkerCount = workCount;
            });

            services.AddHangfireServer(options =>
            {
                options.Queues = new[] { "process-folder", "process-local-folder", "build-autodesk-mirror" };

                int workCount = 1;
                Int32.TryParse(AppSettings.Instance.FolderWorkerCount, out workCount);
                options.WorkerCount = workCount;
            });

            services.AddHangfireServer(options =>
            {
                options.Queues = new[] { "create-local-mirror" };
                options.WorkerCount = 1;
            });

            services.AddHangfireServer(options =>
            {
                options.Queues = new[] { "create-cloud-folders" };
                options.WorkerCount = 1;
            });

            services.AddHangfireServer(options =>
            {
                options.Queues = new[] { "default" };
                options.WorkerCount = 10;
            });

            services.AddHangfireServer(options =>
            {
                options.Queues = new[] { "process-file-download" };
                options.WorkerCount = 10;
            });

            services.AddHangfireServer(options =>
            {
                options.Queues = new[] { "kill-finished" };
                options.WorkerCount = 1;
            });

            services.AddSingleton<ISettingsProvider>((sp) => new Utils.SettingsProvider(sp));

            ThreeLeggedTokenManager.InitializeInstance(AppSettings.Instance, OnTokenRefresh);
            //TwoLeggedManager.InitializeInstance(AppSettings.Instance, null);

            APSSettings.SetFlurSettings(true, true, true);
            _serviceProvider = services.BuildServiceProvider();
            services.AddSingleton<IServiceProvider>(_serviceProvider);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseHangfireDashboard();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
            });

            // check that there aren't existing files which need to be downloaded that didn't finish last application run
            var tempContext = _serviceProvider.GetService<DataContext>();
            var reDownloadFiles = tempContext.BulkDownloadFiles
                .Where(x => x.Status == DownloadFileStatus.Pending || x.Status == DownloadFileStatus.InProgress)
                .Select(x => x.Id)
                .ToList();
            foreach (var fileId in reDownloadFiles)
            {
                BackgroundJob.Enqueue<DownloaderHangfireJobs>(y => y.ProcessFileDownload(fileId));
            }

            var reUploadFiles = tempContext.BulkUploadFiles
                .Where(x => x.Status == JobFileStatus.Pending)
                .Select(x => new { x.Id, x.BulkUploadId })
                .ToList();
            foreach (var fileId in reUploadFiles)
            {
                BackgroundJob.Enqueue<HangfireJobs>(y => y.ProcessFile(fileId.BulkUploadId, fileId.Id));
            }

            var sp = app.ApplicationServices.GetService<IServiceProvider>();
            foreach (var plugin in PluginManager.Instance.Plugins)
            {
                plugin.Configure(app, env, _serviceProvider);
            }
        }
    }
}