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
            try
            {
                _logger.LogInformation("Windows service started");

                await base.StartAsync(cancellationToken);

                _endpointInstance = Endpoint
                    .Start(_endpointConfiguration)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception at StartAsync method, Error: {e}");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _endpointInstance?.Stop().ConfigureAwait(false);

                await base.StopAsync(cancellationToken);

                _logger.LogInformation("Windows service stopped");
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception at StopAsync method, Error: {e}");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at: {DateTimeOffset.Now}");
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override void Dispose()
        {
            // Code implementation
        }
    }
}
