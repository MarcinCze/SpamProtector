using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ProtectorLib.Handlers;
using ProtectorLib.Providers;

using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MarkingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IMessagesHandler messagesHandler;
        private readonly IServiceRunHistoryHandler serviceRunHistoryHandler;
        private readonly IServiceRunScheduleProvider serviceRunScheduleProvider;

        public Worker(
            ILogger<Worker> logger,
            IMessagesHandler messagesHandler,
            IServiceRunHistoryHandler serviceRunHistoryHandler,
            IServiceRunScheduleProvider serviceRunScheduleProvider)
        {
            this.logger = logger;
            this.messagesHandler = messagesHandler;
            this.serviceRunHistoryHandler = serviceRunHistoryHandler;
            this.serviceRunScheduleProvider = serviceRunScheduleProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stopWatch = new Stopwatch();

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                if (serviceRunScheduleProvider.ShouldRun(nameof(MarkingService)))
                {
                    logger.LogInformation("Started operation");
                    stopWatch.Start();

                    string additionalInfo = null;
                    var status = ServiceRunHistoryHandler.ServiceStatus.PROCESSING;

                    await serviceRunHistoryHandler.RegisterStartAsync(nameof(MarkingService), GetType().Assembly.GetName().Version.ToString());

                    try
                    {
                        await messagesHandler.MarkForRemovalAsync();
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
                        await serviceRunHistoryHandler.RegisterFinishAsync(nameof(MarkingService), additionalInfo, status, $"{stopWatch.ElapsedMilliseconds} ms");
                        stopWatch.Reset();
                    }

                    logger.LogInformation("Service run done");
                }

                await Task.Delay(new TimeSpan(0, 0, 30), stoppingToken);
            }
        }
    }
}