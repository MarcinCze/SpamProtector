using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ProtectorLib.Handlers;
using ProtectorLib.Providers;

using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ScanService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IMailboxProvider mailboxProvider;
        private readonly IServiceRunHistoryHandler serviceRunHistoryHandler;
        private readonly IServiceRunScheduleProvider serviceRunScheduleProvider;

        public Worker(
            ILogger<Worker> logger,
            IMailboxProvider mailboxProvider,
            IServiceRunHistoryHandler serviceRunHistoryHandler,
            IServiceRunScheduleProvider serviceRunScheduleProvider)
        {
            this.logger = logger;
            this.mailboxProvider = mailboxProvider;
            this.serviceRunHistoryHandler = serviceRunHistoryHandler;
            this.serviceRunScheduleProvider = serviceRunScheduleProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stopWatch = new Stopwatch();

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                if (serviceRunScheduleProvider.ShouldRun(nameof(ScanService)))
                {
                    logger.LogInformation("Started operation");
                    stopWatch.Start();

                    string additionalInfo = null;
                    var status = ServiceRunHistoryHandler.ServiceStatus.PROCESSING;

                    await serviceRunHistoryHandler.RegisterStartAsync(nameof(ScanService));

                    try
                    {
                        await mailboxProvider.DetectSpamAsync();
                        status = ServiceRunHistoryHandler.ServiceStatus.DONE;
                    }
                    catch (Exception ex)
                    {
                        status = ServiceRunHistoryHandler.ServiceStatus.ERROR;
                        additionalInfo = JsonSerializer.Serialize(new { ex.Message, ex.StackTrace });
                    }
                    finally
                    {
                        stopWatch.Stop();
                        await serviceRunHistoryHandler.RegisterFinishAsync(nameof(ScanService), additionalInfo, status, $"{stopWatch.ElapsedMilliseconds} ms");
                        stopWatch.Reset();
                    }

                    logger.LogInformation("Service run done");
                }

                await Task.Delay(new TimeSpan(0, 0, 30), stoppingToken);
            }
        }


    }
}
