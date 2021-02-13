using ProtectorLib.Models.Enums;

using System.Threading.Tasks;

namespace ProtectorLib.Handlers
{
    public interface IServiceRunHistoryHandler
    {
        Task RegisterStartAsync(string serviceName, string serviceVersion);
        Task RegisterStartAsync(string serviceName, string serviceVersion, string branchName);
        Task RegisterFinishAsync(string serviceName, string additionalData, ServiceStatus endStatus, string executionTime);
        Task RegisterFinishAsync(string serviceName, string branchName, string additionalData, ServiceStatus endStatus, string executionTime);
    }
}
