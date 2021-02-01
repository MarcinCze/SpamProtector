using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public interface IServiceRunScheduleProvider
    {
        Task<bool> ShouldRunAsync(string serviceName);
        Task<bool> ShouldRunAsync(string serviceName, string branchName);
        Task SaveLastRun(string serviceName);
        Task SaveLastRun(string serviceName, string branchName);
    }
}
