using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProtectorLib.Handlers
{
    public interface IMessagesHandler
    {
        Task CatalogMessagesAsync(IEnumerable<Message> messages);
    }
}
