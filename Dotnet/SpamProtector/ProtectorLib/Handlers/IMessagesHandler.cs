using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProtectorLib.Handlers
{
    public interface IMessagesHandler
    {
        Task MarkForRemovalAsync();
        Task CatalogMessagesAsync(IEnumerable<Message> messages);
    }
}
