using MessageQueue.Domain.Entities;
using MessageQueue.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MessageQueue.ClientCommand
{
    internal class Program
    {
        private const string configFiles = "appsettings";
        private const string extension = "json";
        private const string appSection = "AppSettings";
        private const string nServiceBusSection = "NServiceBusSettings";
        private const string connectionName = "PrimaryConnection";

        private static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile($"hostsettings.{extension}", optional: true);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile($"{configFiles}.{extension}", optional: true);
                    configApp.AddJsonFile($"{configFiles}.{hostContext.HostingEnvironment.EnvironmentName}.{extension}", optional: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Classes(hostContext.Configuration.GetSection(appSection));
                    services.Databases(hostContext.Configuration.GetConnectionString(connectionName));
                    services.Services();
                    services.Repositories();

                    NserviceBus.Configuration.Register(hostContext, services,
                        connectionName, nServiceBusSection, appSection, true, typeof(MessageCommandEntity));
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
