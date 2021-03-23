using Microsoft.Extensions.Logging;

using ProtectorLib.Controllers;
using ProtectorLib.Handlers;
using ProtectorLib.Providers;
using ProtectorLib.Services;

using System;
using System.Threading.Tasks;

namespace ScanService
{
    public class Worker : MultiProviderExtendedBackgroundService
    {
        public Worker(
            ILogger<Worker> logger,
            IMailboxController controller,
            IServiceRunHistoryHandler serviceRunHistoryHandler,
            IServiceRunScheduleProvider serviceRunScheduleProvider) : base (logger, controller, serviceRunHistoryHandler, serviceRunScheduleProvider)
        {
        }

        protected override string ServiceName => nameof(ScanService);
        protected override string ServiceVersion => GetType().Assembly.GetName().Version?.ToString();

        protected override TimeSpan StartDelay => ProtectorLib.Configuration.StartDelay.ScanService;

        protected override async Task ExecuteBodyAsync()
        {
            int newSpamCounter = await controller.CurrentMailboxProvider.DetectSpamAsync();
            ServiceResultAdditionalInfo = $"Number of detected new spam mails: {newSpamCounter}";
        }
    }
}