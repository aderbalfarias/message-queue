using HostApp.Domain.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HostApp.WindowsService
{
    public class ConsoleHost : IHostedService
    {
        private readonly ILogger _logger;
        private readonly ITestService _testService;

        public ConsoleHost(ILogger<ConsoleHost> logger, ITestService testService)
        {
            _logger = logger;
            _testService = testService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Core module started");

            try
            {
                _testService.GetAll();
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