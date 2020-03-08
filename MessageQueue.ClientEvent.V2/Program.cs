using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageQueue.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MessageQueue.ClientEvent.V2
{
    public class Program
    {
        private const string appSection = "AppSettings";
        private const string nServiceBusSection = "NServiceBusSettings";
        private const string connectionName = "PrimaryConnection";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    services.Classes(hostContext.Configuration.GetSection(appSection));
                    services.Databases(hostContext.Configuration.GetConnectionString(connectionName));
                    services.Services();
                    services.Repositories();

                    NserviceBus.Configuration
                        .Register(hostContext, services, connectionName, nServiceBusSection, appSection, true);
                })
                .UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
    }
}
