using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ProtectorLib.Data;

namespace ProtectorLib.Handlers
{
    public interface IMessagesHandler
    {
        Task MarkForRemovalAsync();
        Task<IEnumerable<Message>> GetMessagesForRemovalAsync(string mailbox);
        Task<IEnumerable<Message>> GetRemovedMessagesForCheckingAsync();
        

        [Obsolete("IMessagesHandler.CatalogMessagesAsync is obsolete. MessagesService should be used instead")]
        Task<int> CatalogMessagesAsync(IEnumerable<Message> messages);

        [Obsolete("IMessagesHandler.SetMessagesAsPermamentlyRemovedAsync is obsolete. MessagesService should be used instead")]
        Task SetMessagesAsPermamentlyRemovedAsync(IEnumerable<int> removedMsgsIds);

        [Obsolete("IMessagesHandler.MarkMessagesAsRemovedAsync is obsolete. MessagesService should be used instead")]
        Task MarkMessagesAsRemovedAsync(IEnumerable<int> removedMsgIds);
    }
}
