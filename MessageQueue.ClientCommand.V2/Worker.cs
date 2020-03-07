using System;
using System.Threading;
using System.Threading.Tasks;
using MessageQueue.Domain.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace MessageQueue.ClientCommand.V2
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
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
            // Code implementation

            // Perform post-startup activities here
            await _commandService.SendMessageAsync();

            _logger.LogInformation("Windows service started");

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _endpointInstance?.Stop().ConfigureAwait(false);

            _logger.LogInformation("Windows service stopped");

            await base.StopAsync(cancellationToken);
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
