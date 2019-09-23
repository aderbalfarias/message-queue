using HostApp.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HostApp.WindowsService
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: true);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", optional: true);
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Classes(hostContext.Configuration.GetSection("AppSettings"));
                    services.Databases(hostContext.Configuration.GetConnectionString("PrimaryConnection"));
                    services.Services();
                    services.Repositories();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                .UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

            if (isService)
            {
                await builder
                    .ConfigureServices((hostContext, services) 
                        => services.AddSingleton<IHostLifetime, LifetimeEventsServiceBase>())
                    .Build()
                    .RunAsync();
            }
            else
            {
                builder.ConfigureServices((hostContext, services)
                        => services.AddHostedService<ConsoleHost>());
                
                await builder.RunConsoleAsync();
            }
        }
    }
}
