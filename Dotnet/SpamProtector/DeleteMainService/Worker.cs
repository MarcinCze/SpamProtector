using Microsoft.Extensions.Logging;
using ProtectorLib.Handlers;
using ProtectorLib.Providers;
using ProtectorLib.Services;
using System.Threading.Tasks;

namespace DeleteMainService
{
    public class Worker : ExtendedBackgroundService
    {
        private readonly IMailboxProvider mailboxProvider;

        public Worker(
            ILogger<Worker> logger,
            IMailboxProvider mailboxProvider,
            IServiceRunHistoryHandler serviceRunHistoryHandler,
            IServiceRunScheduleProvider serviceRunScheduleProvider) : base (logger, serviceRunHistoryHandler, serviceRunScheduleProvider)
        {
            this.mailboxProvider = mailboxProvider;
        }

        protected override string ServiceName => nameof(DeleteMainService);
        protected override string ServiceVersion => GetType().Assembly.GetName().Version?.ToString();

        protected override async Task ExecuteBodyAsync()
        {
            (int countBefore, int countAfter) = await mailboxProvider.DeleteMessagesAsync();
            ServiceResultAdditionalInfo = $"BEFORE: {countBefore} AFTER: {countAfter}";
        }
    }
}
