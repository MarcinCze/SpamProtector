using Microsoft.Extensions.Logging;
using ProtectorLib.Handlers;
using ProtectorLib.Providers;
using ProtectorLib.Services;
using System.Threading.Tasks;

namespace ScanService
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

        protected override string ServiceName => nameof(ScanService);
        protected override string ServiceVersion => GetType().Assembly.GetName().Version?.ToString();

        protected override async Task ExecuteBodyAsync()
        {
            int newSpamCounter = await mailboxProvider.DetectSpamAsync();
            ServiceResultAdditionalInfo = $"Number of detected new spam mails: {newSpamCounter}";
        }
    }
}