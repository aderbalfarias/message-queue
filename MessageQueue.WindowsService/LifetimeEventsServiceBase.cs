using HostApp.Domain.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace HostApp.WindowsService
{
    internal class LifetimeEventsServiceBase : ServiceBase, IHostLifetime
    {
        private readonly ILogger _logger;
        private readonly IApplicationLifetime _appLifetime;
        private readonly ITestService _testSercice;

        public LifetimeEventsServiceBase(
            ILogger<LifetimeEventsServiceBase> logger,
            IApplicationLifetime appLifetime,
            ITestService testService)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _testSercice = testService;
        }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            new Thread(Run).Start();

            return Task.CompletedTask;
        }

        private void Run()
        {
            try
            {
                _logger.LogInformation("Windows service running");

                Run(this); // This blocks until the service is stopped.
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception at Run method, Error: {ex}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Stop();

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Called by base.Run when the service is ready to start.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            _logger.LogInformation("Windows service started");

            // Perform post-startup activities here
            _testSercice.GetAll();

            base.OnStart(args);
        }

        /// <summary>
        ///     Called by base.Stop This may be called multiple times by service Stop
        ///     StopApplication uses a CancellationTokenSource and prevents any recursion.
        /// </summary>
        protected override void OnStop()
        {
            _logger.LogInformation("Windows service stopped");

            _appLifetime.StopApplication();

            base.OnStop();
        }
    }
}
