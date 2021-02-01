using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;

using ProtectorLib.Configuration;
using ProtectorLib.Handlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public abstract class BaseMailboxProvider : IMailboxProvider
    {
        protected readonly MailboxConfig mailboxConfig;
        protected readonly ServicesConfig servicesConfig;
        protected readonly IMessagesHandler messagesHandler;
		protected readonly IDateTimeProvider dateTimeProvider;

        protected BaseMailboxProvider(
			MailboxConfig mailboxConfig, 
			ServicesConfig servicesConfig, 
			IMessagesHandler messagesHandler, 
			IDateTimeProvider dateTimeProvider)
        {
            this.mailboxConfig = mailboxConfig;
            this.servicesConfig = servicesConfig;
            this.messagesHandler = messagesHandler;
			this.dateTimeProvider = dateTimeProvider;
        }

		protected string MailBoxName { get; set; }

		protected virtual DateTime DeliveredAfterDate => dateTimeProvider.CurrentTime.Date.AddDays(-servicesConfig.CatalogDaysToCheck);
		protected virtual DateTime DeliveredAfterDateScan => dateTimeProvider.CurrentTime.Date.AddDays(-servicesConfig.ScanDaysToCheck);

		public virtual async Task<int> CatalogAsync()
        {
			var messages = new List<Message>();

			using (var client = new ImapClient())
			{
				await client.ConnectAsync(mailboxConfig.Url, mailboxConfig.Port, SecureSocketOptions.SslOnConnect);
				await client.AuthenticateAsync(mailboxConfig.UserName, mailboxConfig.Password);

				var junkFolder = await GetJunkFolderAsync(client);
				await junkFolder.OpenAsync(FolderAccess.ReadOnly);
				var uids = await junkFolder.SearchAsync(SearchQuery.DeliveredAfter(DeliveredAfterDate));

				foreach (var uid in uids)
				{
					var message = await junkFolder.GetMessageAsync(uid);

					messages.Add(new Message
					{
						ImapUid = (int)uid.Id,
						Mailbox = MailBoxName,
						Recipient = message.To.Mailboxes.FirstOrDefault()?.Address,
						Sender = message.From.Mailboxes.FirstOrDefault()?.Address,
						Subject = message.Subject,
						Content = string.IsNullOrEmpty(message.TextBody) ? "TEXT BODY NOT PROVIDED. ONLY HTML" : message.TextBody,
						ReceivedTime = message.Date.DateTime
					});
				}

				await client.DisconnectAsync(true);
			}

			return await messagesHandler.CatalogMessagesAsync(messages);
		}

		public virtual async Task<(int countBefore, int countAfter)> DeleteMessagesAsync()
        {
			var messagesToRemove = await messagesHandler.GetMessagesForRemovalAsync(MailBoxName);
			var messagesRemoved = new List<int>();

			using (var client = new ImapClient())
			{
				await client.ConnectAsync(mailboxConfig.Url, mailboxConfig.Port, SecureSocketOptions.SslOnConnect);
				await client.AuthenticateAsync(mailboxConfig.UserName, mailboxConfig.Password);

				var junkFolder = await GetJunkFolderAsync(client);
				await junkFolder.OpenAsync(FolderAccess.ReadWrite);
				int countBefore = junkFolder.Count;

				if (!messagesToRemove.Any())
					return (countBefore, countBefore);

				foreach (var message in messagesToRemove)
                {
					try
                    {
						UniqueId uid = new UniqueId((uint)message.ImapUid);
						var mail = await junkFolder.GetMessageAsync(uid);
						await junkFolder.AddFlagsAsync(uid, MessageFlags.Deleted, true);
						messagesRemoved.Add(message.Id);
					}
                    catch (Exception ex)
                    { }
                }

				await junkFolder.ExpungeAsync();
				int countAfter = junkFolder.Count;
				await client.DisconnectAsync(true);
				await messagesHandler.MarkMessagesAsRemovedAsync(messagesRemoved);

				return (countBefore, countAfter);
			}
		}

		public virtual Task<int> DetectSpamAsync() => throw new NotImplementedException();

        protected virtual Task<IMailFolder> GetJunkFolderAsync(ImapClient imapClient) => throw new NotImplementedException();
    }
}
