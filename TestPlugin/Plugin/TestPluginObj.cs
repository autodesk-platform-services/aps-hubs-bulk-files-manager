using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NPOI.SS.Formula.Functions;
using PluginBase;
using PluginBase.Models;

namespace TestPlugin.Plugin
{
    public class TestPluginObj : IJobExecutionPlugin
    {
        public TestPluginObj()
        {
            if (_instance != null)
                throw new NotSupportedException($"Only one instance of the TestPluginObj plugin is supported");
            _instance = this;
        }
        public static TestPluginObj Instance => _instance;
        private static TestPluginObj _instance;

        public IServiceProvider ServiceProvider { get; private set; }


        public void Configure(IApplicationBuilder app, IHostEnvironment env, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            ISettingsProvider sp = ServiceProvider.GetService<ISettingsProvider>()!;


        }

        public void ConfigureServices(IServiceCollection services)
        {
        }


    }
}