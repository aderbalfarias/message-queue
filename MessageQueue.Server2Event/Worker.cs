using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessageQueue.Server2Event
{
    public class Worker : BackgroundService
    {
        private IEndpointInstance _endpointInstance;

        private readonly ILogger<Worker> _logger;
        private readonly EndpointConfiguration _endpointConfiguration;

        public Worker
        (
            ILogger<Worker> logger,
            EndpointConfiguration endpointConfiguration
        )
        {
            _logger = logger;
            _endpointConfiguration = endpointConfiguration;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Windows service started");

            await base.StartAsync(cancellationToken);

            _endpointInstance = Endpoint
                .Start(_endpointConfiguration)
                .GetAwaiter()
                .GetResult();
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
