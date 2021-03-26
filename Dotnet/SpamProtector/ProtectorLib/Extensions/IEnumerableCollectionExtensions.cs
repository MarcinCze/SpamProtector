using MailKit;

using ProtectorLib.Data;
using ProtectorLib.Models;

using System;
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

        public static IEnumerable<EmailDTO> ConvertToDto(this IEnumerable<Message> collection, DateTime versionUpdateTime) =>
            collection.Select(message => new Models.EmailDTO
            {
                Id = message.Id,
                CatalogTime = message.CatalogTime,
                Content = message.Content,
                ImapUid = message.ImapUid,
                IsRemoved = message.IsRemoved,
                Mailbox = message.Mailbox,
                PlannedRemoveTime = message.PlannedRemoveTime,
                ReceivedTime = message.ReceivedTime,
                Recipient = message.Recipient,
                RemoveTime = message.RemoveTime,
                Sender = message.Sender,
                Subject = message.Subject,
                VersionUpdateTime = versionUpdateTime
            }).ToList();
    }
}
