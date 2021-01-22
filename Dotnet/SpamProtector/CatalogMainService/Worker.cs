using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ProtectorLib.Handlers;
using ProtectorLib.Providers;

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogMainService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IMailboxProvider mailboxProvider;
        private readonly IServiceRunHistoryHandler serviceRunHistoryHandler;
        private readonly IServiceRunScheduleProvider serviceRunScheduleProvider;

        public Worker(
            IMailboxProvider mailboxProvider, 
            ILogger<Worker> logger, 
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
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
                if (serviceRunScheduleProvider.ShouldRun(nameof(CatalogMainService)))
                {
                    await serviceRunHistoryHandler.RegisterStartAsync(nameof(CatalogMainService));
                    try
                    {
                        await mailboxProvider.CatalogAsync();
                        await serviceRunHistoryHandler.RegisterFinishAsync(nameof(CatalogMainService), null, ServiceRunHistoryHandler.ServiceStatus.DONE);
                    }
                    catch (Exception ex)
                    {
                        await serviceRunHistoryHandler.RegisterFinishAsync(
                            nameof(CatalogMainService),
                            JsonSerializer.Serialize(new { ex.Message, ex.StackTrace }),
                            ServiceRunHistoryHandler.ServiceStatus.ERROR
                            );
                    }

                    logger.LogInformation("Service run done");
                }
                
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
