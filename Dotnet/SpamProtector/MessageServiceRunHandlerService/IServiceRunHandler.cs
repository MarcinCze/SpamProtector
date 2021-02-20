using System.Threading.Tasks;

using ProtectorLib.Models;

namespace MessageServiceRunHandlerService
{
    public interface IServiceRunHandler
    {
        Task SaveAsync(ServiceRunDTO message);
    }
}
