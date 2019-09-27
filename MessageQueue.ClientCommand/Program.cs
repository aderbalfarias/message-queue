using MessageQueue.Domain.Entities;
using MessageQueue.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence.Sql;
using Serilog;
using System;
using System.Data.SqlClient;
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
        private const string appSettings = "AppSettings";
        private const string nServiceBusSettings = "NServiceBusSettings";
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
                    services.Classes(hostContext.Configuration.GetSection(appSettings));
                    services.Databases(hostContext.Configuration.GetConnectionString(connectionName));
                    services.Services();
                    services.Repositories();

                    NServiceBusConfig(hostContext, services);
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

        private static Task NServiceBusConfig(HostBuilderContext hostContext, IServiceCollection services)
        {
            var serviceBusSettings = hostContext.Configuration.GetSection(nServiceBusSettings).Get<NServiceBusSettings>();

            var endpointConfiguration = new EndpointConfiguration(serviceBusSettings.ProjectEndpoint);

            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(hostContext.Configuration.GetConnectionString(connectionName));
            transport.Transactions(TransportTransactionMode.SendsAtomicWithReceive);

            endpointConfiguration.AutoSubscribe();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UsePersistence<SqlPersistence>();
            endpointConfiguration.UseContainer<ServicesBuilder>(c => c.ExistingServices(services));
            endpointConfiguration.SendHeartbeatTo(serviceBusSettings.SendFailedMessagesTo);
            endpointConfiguration.AuditProcessedMessagesTo(serviceBusSettings.AuditProcessedMessagesTo);
            endpointConfiguration.SendFailedMessagesTo(serviceBusSettings.SendFailedMessagesTo);

            //var recoverability = endpointConfiguration.Recoverability();
            //recoverability.Immediate(
            //    immediate =>
            //    {
            //        immediate.NumberOfRetries(serviceBusSettings.NumberOfRetries);
            //    });
            //recoverability.Delayed(
            //    delayed =>
            //    {
            //        delayed.NumberOfRetries(serviceBusSettings.NumberOfRetries);
            //        delayed.TimeIncrease(TimeSpan.FromSeconds(serviceBusSettings.RecoverabilityTimeIncreaseInSeconds));
            //    });

            //var metrics = endpointConfiguration.EnableMetrics();
            //metrics.SendMetricDataToServiceControl(nServiceBusSettings.SendMetricDataToServiceControl,
            //    TimeSpan.FromMilliseconds(nServiceBusSettings.SendMetricDataToServiceControlIntervalInMilliseconds));

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(
                connectionBuilder: () =>
                {
                    return new SqlConnection(hostContext.Configuration.GetConnectionString(connectionName));
                });

            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.CacheFor(TimeSpan.FromMinutes(serviceBusSettings.SubscriptionCacheForInMinutes));

            var defaultFactory = LogManager.Use<DefaultFactory>();
            defaultFactory.Directory(serviceBusSettings.PathToLog);

            endpointConfiguration.RegisterComponents(
                registration: configureComponents =>
                {
                    configureComponents.RegisterSingleton(hostContext.Configuration.GetSection(appSettings).Get<AppSettings>());
                });

            var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            services.AddSingleton(endpointInstance);
            services.AddSingleton<IMessageSession>(endpointInstance);


            return Task.CompletedTask;
        }
    }
}
