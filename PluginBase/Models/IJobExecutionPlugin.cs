using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace PluginBase.Models
{
    public interface IJobExecutionPlugin
    {

        void ConfigureServices(IServiceCollection services);

        void Configure(IApplicationBuilder app, IHostEnvironment env, IServiceProvider serviceProvider);

    }
}