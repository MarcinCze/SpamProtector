using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ProtectorLib.Handlers;
using ProtectorLib.Providers;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace CatalogMainService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IMailboxProvider mailboxProvider;
        private readonly IServiceRunHistoryHandler serviceRunHistoryHandler;

        public Worker(IMailboxProvider mailboxProvider, ILogger<Worker> logger, IServiceRunHistoryHandler serviceRunHistoryHandler)
        {
            this.logger = logger;
            this.mailboxProvider = mailboxProvider;
            this.serviceRunHistoryHandler = serviceRunHistoryHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
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

                logger.LogInformation("Done");
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
