using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessageQueue.Server2Event
{
    public class ConsoleHost : IHostedService
    {
        private readonly ILogger _logger;
        private readonly EndpointConfiguration _endpointConfiguration;
        private IEndpointInstance _endpointInstance;

        public ConsoleHost(
            ILogger<ConsoleHost> logger,
            EndpointConfiguration endpointConfiguration)
        {
            _logger = logger;
            _endpointConfiguration = endpointConfiguration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Core module started");

            try
            {
                _endpointInstance = Endpoint
                    .Start(_endpointConfiguration)
                    .GetAwaiter()
                    .GetResult();
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