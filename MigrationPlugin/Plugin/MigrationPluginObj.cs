using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NPOI.SS.Formula.Functions;
using PluginBase;
using PluginBase.Models;

namespace MigrationPlugin.Plugin
{
    public class MigrationPluginObj : IJobExecutionPlugin
    {
        public MigrationPluginObj()
        {
            if (_instance != null)
                throw new NotSupportedException($"Only one instance of the MigrationPlugin plugin is supported");
            _instance = this;
        }
        public static MigrationPluginObj Instance => _instance; 
        private static MigrationPluginObj _instance;

        public IServiceProvider ServiceProvider { get; private set; }


        public void Configure(IApplicationBuilder app, IHostEnvironment env, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            ISettingsProvider sp = ServiceProvider.GetService<ISettingsProvider>()!;
            sp.RegisterGlobalSetting("OutputFolder", null);

        }

        public void ConfigureServices(IServiceCollection services)
        {
        }


    }
}