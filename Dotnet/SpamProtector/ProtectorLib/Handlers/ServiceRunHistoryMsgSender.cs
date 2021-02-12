using ProtectorLib.Enums;
using ProtectorLib.Messaging;
using ProtectorLib.Providers;

using System.Threading.Tasks;


namespace ProtectorLib.Handlers
{
    public class ServiceRunHistoryMsgSender : IServiceRunHistoryHandler
    {
        private readonly IMessagingService messagingService;
        private readonly IDateTimeProvider dateTimeProvider;

        public ServiceRunHistoryMsgSender(IMessagingService messagingService, IDateTimeProvider dateTimeProvider)
        {
            this.messagingService = messagingService;
            this.dateTimeProvider = dateTimeProvider;
        }

        public async Task RegisterFinishAsync(string serviceName, string additionalData, ServiceStatus endStatus, string executionTime)
        {
            await RegisterFinishAsync(serviceName, null, additionalData, endStatus, executionTime);
        }

        public async Task RegisterFinishAsync(string serviceName, string branchName, string additionalData, ServiceStatus endStatus, string executionTime)
        {
            await Task.Run(() =>
            {
                messagingService.SendMessage(new Models.ServiceRun()
                {
                    ServiceName = serviceName,
                    Branch = branchName,
                    Status = endStatus.ToString(),
                    EndTime = dateTimeProvider.CurrentTime,
                    Information = additionalData.Length < 999 ? additionalData : additionalData.Substring(0, 999),
                    ExecutionTime = executionTime
                });
            });
        }

        public async Task RegisterStartAsync(string serviceName, string serviceVersion)
        {
            await RegisterStartAsync(serviceName, serviceVersion, null);
        }

        public async Task RegisterStartAsync(string serviceName, string serviceVersion, string branchName)
        {
            await Task.Run(() =>
            {
                messagingService.SendMessage(new Models.ServiceRun()
                {
                    ServiceName = serviceName,
                    Branch = branchName,
                    ServiceVersion = serviceVersion,
                    Status = ServiceStatus.PROCESSING.ToString(),
                    StartTime = dateTimeProvider.CurrentTime
                });
            });    
        }
    }
}
