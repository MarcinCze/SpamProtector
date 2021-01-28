using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ProtectorLib.Handlers;
using ProtectorLib.Providers;

using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DeleteMainService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IServiceRunHistoryHandler serviceRunHistoryHandler;
        private readonly IServiceRunScheduleProvider serviceRunScheduleProvider;
        private readonly IMailboxProvider mailboxProvider;

        public Worker(
            ILogger<Worker> logger,
            IMailboxProvider mailboxProvider,
            IServiceRunHistoryHandler serviceRunHistoryHandler,
            IServiceRunScheduleProvider serviceRunScheduleProvider)
        {
            this.logger = logger;
            this.mailboxProvider = mailboxProvider;
            this.serviceRunScheduleProvider = serviceRunScheduleProvider;
            this.serviceRunHistoryHandler = serviceRunHistoryHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stopWatch = new Stopwatch();

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                if (serviceRunScheduleProvider.ShouldRun(nameof(DeleteMainService)))
                {
                    logger.LogInformation("Started operation");
                    stopWatch.Start();

                    string additionalInfo = null;
                    var status = ServiceRunHistoryHandler.ServiceStatus.PROCESSING;

                    await serviceRunHistoryHandler.RegisterStartAsync(nameof(DeleteMainService), GetType().Assembly.GetName().Version.ToString());

                    try
                    {
                        (int countBefore, int countAfter) = await mailboxProvider.DeleteMessagesAsync();
                        status = ServiceRunHistoryHandler.ServiceStatus.DONE;
                        additionalInfo = $"BEFORE: {countBefore} AFTER: {countAfter}";
                    }
                    catch (Exception ex)
                    {
                        status = ServiceRunHistoryHandler.ServiceStatus.ERROR;
                        additionalInfo = JsonSerializer.Serialize(new { ex.Message, ex.StackTrace });
                    }
                    finally
                    {
                        stopWatch.Stop();
                        await serviceRunHistoryHandler.RegisterFinishAsync(nameof(DeleteMainService), additionalInfo, status, $"{stopWatch.ElapsedMilliseconds} ms");
                        stopWatch.Reset();
                    }

                    logger.LogInformation("Service run done");
                }

                await Task.Delay(new TimeSpan(0, 0, 30), stoppingToken);
            }
        }
    }
}
