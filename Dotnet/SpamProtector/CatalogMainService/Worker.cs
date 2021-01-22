using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ProtectorLib.Providers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogMainService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IMailboxProvider mailboxProvider;

        public Worker(IMailboxProvider mailboxProvider, ILogger<Worker> logger)
        {
            this.logger = logger;
            this.mailboxProvider = mailboxProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await mailboxProvider.CatalogAsync();
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
