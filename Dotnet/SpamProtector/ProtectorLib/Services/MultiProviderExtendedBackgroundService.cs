using Microsoft.Extensions.Logging;

using ProtectorLib.Controllers;
using ProtectorLib.Handlers;
using ProtectorLib.Models.Enums;
using ProtectorLib.Providers;

using System.Threading.Tasks;

namespace ProtectorLib.Services
{
    public abstract class MultiProviderExtendedBackgroundService : ExtendedBackgroundService
    {
        protected readonly IMailboxController controller;

        protected MultiProviderExtendedBackgroundService(
            ILogger logger,
            IMailboxController controller,
            IServiceRunHistoryHandler serviceRunHistoryHandler,
            IServiceRunScheduleProvider serviceRunScheduleProvider) : base (logger, serviceRunHistoryHandler, serviceRunScheduleProvider)
        {
            this.controller = controller;
        }

        protected override Task<bool> ShouldItRunAsync() =>
            serviceRunScheduleProvider.ShouldRunAsync(ServiceName, controller.CurrentMailboxProvider.MailBoxName);

        protected override async Task SaveStartAsync()
            => await serviceRunHistoryHandler.RegisterStartAsync(ServiceName, ServiceVersion, controller.CurrentMailboxProvider.MailBoxName);

        protected override async Task SaveFinishAsync(ServiceStatus status, string executionTime)
            => await serviceRunHistoryHandler.RegisterFinishAsync(ServiceName, controller.CurrentMailboxProvider.MailBoxName, ServiceResultAdditionalInfo, status, executionTime);

        protected override async Task SaveLastRunAsync()
            => await serviceRunScheduleProvider.SaveLastRunAsync(ServiceName, controller.CurrentMailboxProvider.MailBoxName);

        protected override void FinishActions()
        {
            controller.SetNextProvider();
            logger.LogInformation($"FinishAction. New current provider: {controller.CurrentMailboxProvider.MailBoxName}");
        }
    }
}
