using Microsoft.Extensions.Logging;
using ProtectorLib.Handlers;
using ProtectorLib.Providers;
using ProtectorLib.Services;
using System.Threading.Tasks;

namespace CatalogMainService
{
    public class Worker : ExtendedBackgroundService
    {
        private readonly IMailboxProvider mailboxProvider;

        public Worker(
            IMailboxProvider mailboxProvider, 
            ILogger<Worker> logger, 
            IServiceRunHistoryHandler serviceRunHistoryHandler,
            IServiceRunScheduleProvider serviceRunScheduleProvider) : base (logger, serviceRunHistoryHandler, serviceRunScheduleProvider)
        {
            this.mailboxProvider = mailboxProvider;
        }

        protected override string ServiceName => nameof(CatalogMainService);

        protected override string ServiceVersion => GetType().Assembly.GetName().Version?.ToString();

        protected override async Task ExecuteBodyAsync()
        {
            int msgInserted = await mailboxProvider.CatalogAsync();
            ServiceResultAdditionalInfo = $"New messages in catalog: {msgInserted}";
        }
    }
}
