using Microsoft.Extensions.Logging;
using ProtectorLib.Controllers;
using ProtectorLib.Handlers;
using ProtectorLib.Providers;
using ProtectorLib.Services;
using System;
using System.Threading.Tasks;

namespace DeleteService
{
    public class Worker : MultiProviderExtendedBackgroundService
    {
        public Worker(
            ILogger<Worker> logger,
            IMailboxController controller,
            IServiceRunHistoryHandler serviceRunHistoryHandler,
            IServiceRunScheduleProvider serviceRunScheduleProvider) : base(logger, controller, serviceRunHistoryHandler, serviceRunScheduleProvider)
        { }

        protected override string ServiceName => nameof(DeleteService);
        protected override string ServiceVersion => GetType().Assembly.GetName().Version?.ToString();
        protected override TimeSpan StartDelay => ProtectorLib.Configuration.StartDelay.DeleteService;

        protected override async Task ExecuteBodyAsync()
        {
            (int countBefore, int countAfter) = await controller.CurrentMailboxProvider.DeleteMessagesAsync();
            ServiceResultAdditionalInfo = $"BEFORE: {countBefore} AFTER: {countAfter}";
        }
    }
}