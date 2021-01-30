using Microsoft.Extensions.Logging;
using ProtectorLib.Handlers;
using ProtectorLib.Providers;
using ProtectorLib.Services;
using System.Threading.Tasks;

namespace MarkingService
{
    public class Worker : ExtendedBackgroundService
    {
        private readonly IMessagesHandler messagesHandler;

        public Worker(
            ILogger<Worker> logger,
            IMessagesHandler messagesHandler,
            IServiceRunHistoryHandler serviceRunHistoryHandler,
            IServiceRunScheduleProvider serviceRunScheduleProvider) : base (logger, serviceRunHistoryHandler, serviceRunScheduleProvider)
        {
            this.messagesHandler = messagesHandler;
        }

        protected override string ServiceName => nameof(MarkingService);
        protected override string ServiceVersion => GetType().Assembly.GetName().Version?.ToString();

        protected override async Task ExecuteBodyAsync()
        {
            await messagesHandler.MarkForRemovalAsync();
        }
    }
}