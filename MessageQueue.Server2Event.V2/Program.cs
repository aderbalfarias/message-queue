using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MessageQueue.Server2Event.V2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    services.Classes(hostContext.Configuration.GetSection(appSection));
                    services.Databases(hostContext.Configuration.GetConnectionString(connectionName));
                    services.Services();
                    services.Repositories();

                    NserviceBus.Configuration
                        .Register(hostContext, services, connectionName, nServiceBusSection,
                            appSection, messageTypePublisher: typeof(MessageEventEntity));
                })
                .UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
    }
}
