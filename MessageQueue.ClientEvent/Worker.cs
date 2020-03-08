using MessageQueue.Domain.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessageQueue.ClientEvent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEventService _eventService;
        private readonly IEndpointInstance _endpointInstance;

        public Worker
        (
            ILogger<Worker> logger,
            IEventService eventService,
            IEndpointInstance endpointInstance
        )
        {
            _logger = logger;
            _eventService = eventService;
            _endpointInstance = endpointInstance;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Windows service started");

            await base.StartAsync(cancellationToken);

            await _eventService.PublishMessageAsync();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _endpointInstance?.Stop().ConfigureAwait(false);

            await base.StopAsync(cancellationToken);

            _logger.LogInformation("Windows service stopped");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override void Dispose()
        {
            // Code implementation
        }
    }
}
