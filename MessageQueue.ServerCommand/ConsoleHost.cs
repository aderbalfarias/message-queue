using MessageQueue.Domain.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessageQueue.ServerCommand
{
    public class ConsoleHost : IHostedService
    {
        private readonly ILogger _logger;
        private readonly ICommandService _commandService;
        private readonly IEndpointInstance _endpointInstance;

        public ConsoleHost(
            ILogger<ConsoleHost> logger,
            ICommandService commandService,
            IEndpointInstance endpointInstance)
        {
            _logger = logger;
            _commandService = commandService;
            _endpointInstance = endpointInstance;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Core module started");

            try
            {
                _commandService.SendMessageAsync();
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