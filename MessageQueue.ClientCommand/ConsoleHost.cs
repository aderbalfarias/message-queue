using MessageQueue.Domain.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessageQueue.ClientCommand
{
    public class ConsoleHost : IHostedService
    {
        private readonly ILogger _logger;
        private readonly ICommandService _commandService;

        public ConsoleHost(ILogger<ConsoleHost> logger, ICommandService commandService)
        {
            _logger = logger;
            _commandService = commandService;
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

            return Task.CompletedTask;
        }
    }
}