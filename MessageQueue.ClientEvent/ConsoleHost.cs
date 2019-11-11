using MessageQueue.Domain.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessageQueue.ClientEvent
{
    public class ConsoleHost : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IEventService _eventService;
        private readonly IEndpointInstance _endpointInstance;

        public ConsoleHost(
            ILogger<ConsoleHost> logger,
            IEventService eventService,
            IEndpointInstance endpointInstance)
        {
            _logger = logger;
            _eventService = eventService;
            _endpointInstance = endpointInstance;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Core module started");

            try
            {
                _eventService.PublishMessageAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception at ConsoleHost, Error: {e}");
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Core module stopped");

            _endpointInstance?.Stop().ConfigureAwait(false);

            return Task.CompletedTask;
        }
    }
}