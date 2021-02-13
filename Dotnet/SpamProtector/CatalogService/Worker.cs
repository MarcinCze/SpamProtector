using Microsoft.Extensions.Logging;
using ProtectorLib.Handlers;
using ProtectorLib.Providers;
using ProtectorLib.Models.Enums;
using ProtectorLib.Services;
using System.Threading.Tasks;
using ProtectorLib.Controllers;
using System;

namespace CatalogService
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

        protected override string ServiceName => nameof(CatalogService);
        protected override string ServiceVersion => GetType().Assembly.GetName().Version?.ToString();
        protected override TimeSpan StartDelay => ProtectorLib.Configuration.StartDelay.CatalogService;

        protected override async Task ExecuteBodyAsync()
        {
            int msgInserted = await controller.CurrentMailboxProvider.CatalogAsync();
            ServiceResultAdditionalInfo = $"New messages in catalog: {msgInserted}";
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
        }
    }
}