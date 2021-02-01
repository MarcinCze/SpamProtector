using System.Threading.Tasks;

namespace ProtectorLib.Handlers
{
    public interface IServiceRunHistoryHandler
    {
        Task RegisterStartAsync(string serviceName, string serviceVersion);
        Task RegisterFinishAsync(string serviceName, string additionalData, ServiceRunHistoryHandler.ServiceStatus endStatus, string executionTime);
    }
}
