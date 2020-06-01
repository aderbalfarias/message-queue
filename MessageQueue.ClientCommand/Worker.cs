using MessageQueue.Domain.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessageQueue.ClientCommand
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ICommandService _commandService;
        private readonly IEndpointInstance _endpointInstance;

        public Worker
        (
            ILogger<Worker> logger,
            ICommandService commandService,
            IEndpointInstance endpointInstance
        )
        {
            _logger = logger;
            _commandService = commandService;
            _endpointInstance = endpointInstance;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Windows service started");

                await base.StartAsync(cancellationToken);

                await _commandService.SendMessageAsync();
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
