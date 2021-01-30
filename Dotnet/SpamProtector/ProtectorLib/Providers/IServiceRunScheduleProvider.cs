using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public interface IServiceRunScheduleProvider
    {
        Task<bool> ShouldRunAsync(string serviceName);
        Task SaveLastRun(string serviceName);
    }
}
