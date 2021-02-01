using Microsoft.Extensions.Logging;
using ProtectorLib.Handlers;
using ProtectorLib.Providers;
using ProtectorLib.Services;
using System.Threading.Tasks;
using ProtectorLib.Controllers;

namespace DeleteService
{
    public class Worker : ExtendedBackgroundService
    {
        private readonly IMailboxController controller;

        public Worker(
            ILogger<Worker> logger,
            IMailboxController controller,
            IServiceRunHistoryHandler serviceRunHistoryHandler,
            IServiceRunScheduleProvider serviceRunScheduleProvider) : base(logger, serviceRunHistoryHandler, serviceRunScheduleProvider)
        {
            this.controller = controller;
        }

        protected override string ServiceName => nameof(DeleteService);
        protected override string ServiceVersion => GetType().Assembly.GetName().Version?.ToString();

        protected override async Task ExecuteBodyAsync()
        {
            (int countBefore, int countAfter) = await controller.CurrentMailboxProvider.DeleteMessagesAsync();
            ServiceResultAdditionalInfo = $"BEFORE: {countBefore} AFTER: {countAfter}";
        }

        protected override Task<bool> ShouldItRunAsync(string serviceName, string branchName = null) =>
            serviceRunScheduleProvider.ShouldRunAsync(ServiceName, controller.CurrentMailboxProvider.MailBoxName);

        protected override void FinishActions()
        {
            controller.SetNextProvider();
        }
    }
}