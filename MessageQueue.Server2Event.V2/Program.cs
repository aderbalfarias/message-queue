using MessageQueue.Domain.Entities;
using MessageQueue.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MessageQueue.Server2Event.V2
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
