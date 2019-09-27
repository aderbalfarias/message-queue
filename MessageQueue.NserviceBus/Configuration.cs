﻿using MessageQueue.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence.Sql;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MessageQueue.NserviceBus
{
    public class Configuration
    {
        /// <summary>
        ///     Method to create initial confiration for NServiceBus endpoints
        /// </summary>
        /// <param name="hostContext"></param>
        /// <param name="services"></param>
        /// <param name="connectionName"></param>
        /// <param name="nServiceBusSection"></param>
        /// <param name="appSection"></param>
        /// <param name="endpointStart"></param>
        /// <param name="messageTypeRoute"></param>
        /// <param name="messageTypePublisher"></param>
        /// <returns></returns>
        public static Task Register(
            HostBuilderContext hostContext, 
            IServiceCollection services,
            string connectionName,
            string nServiceBusSection,
            string appSection,
            bool endpointStart = false,
            Type messageTypeRoute = null,
            Type messageTypePublisher = null)
        {
            var serviceBusSettings = hostContext.Configuration
                .GetSection(nServiceBusSection)
                .Get<NServiceBusSettings>();

            var endpointConfiguration = new EndpointConfiguration(serviceBusSettings.ProjectEndpoint);

            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(hostContext.Configuration.GetConnectionString(connectionName));
            transport.Transactions(TransportTransactionMode.SendsAtomicWithReceive);

            if (messageTypeRoute != null && !string.IsNullOrEmpty(serviceBusSettings.RouteToEndpoint))
                transport.Routing().RouteToEndpoint(messageTypeRoute, serviceBusSettings.RouteToEndpoint);
            
            if (messageTypePublisher != null && !string.IsNullOrEmpty(serviceBusSettings.SubscribeToEndpoint))
                transport.Routing().RegisterPublisher(messageTypePublisher, serviceBusSettings.SubscribeToEndpoint);

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
                    configureComponents.RegisterSingleton(hostContext.Configuration.GetSection(appSection).Get<AppSettings>());
                });

            if (endpointStart)
            {
                var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

                services.AddSingleton(endpointInstance);
                services.AddSingleton<IMessageSession>(endpointInstance);
            }
            else
            {
                services.AddSingleton(endpointConfiguration);
            }           

            return Task.CompletedTask;
        }
    }
}