using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public interface IServiceRunScheduleProvider
    {
        Task<bool> ShouldRunAsync(string serviceName);
        Task<bool> ShouldRunAsync(string serviceName, string branchName);
        Task SaveLastRunAsync(string serviceName);
        Task SaveLastRunAsync(string serviceName, string branchName);
    }
}
