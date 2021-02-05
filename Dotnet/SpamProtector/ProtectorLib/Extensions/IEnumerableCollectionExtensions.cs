using MailKit;

using System.Collections.Generic;
using System.Linq;

namespace ProtectorLib.Extensions
{
    public static class IEnumerableCollectionExtensions
    {
        public static IList<UniqueId> GetUniqueIds(this IEnumerable<Message> collection) =>
            collection.Select(item => new UniqueId((uint)item.ImapUid)).ToList();

        public static IEnumerable<int> GetIds(this IEnumerable<Message> collection) =>
            collection.Select(item => item.Id);
    }
}
