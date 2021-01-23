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

        public BaseMailboxProvider(MailboxConfig mailboxConfig, ServicesConfig servicesConfig, IMessagesHandler messagesHandler)
        {
            this.mailboxConfig = mailboxConfig;
            this.servicesConfig = servicesConfig;
            this.messagesHandler = messagesHandler;
        }

		protected string MailBoxName { get; set; }

		protected virtual DateTime DeliveredAfterDate => DateTime.Now.Date.AddDays(-servicesConfig.CatalogDaysToCheck);
		protected virtual DateTime DeliveredAfterDateScan => DateTime.Now.Date.AddDays(-servicesConfig.ScanDaysToCheck);

		public async virtual Task CatalogAsync()
        {
			var messages = new List<Message>();

			using (var client = new ImapClient())
			{
				await client.ConnectAsync(mailboxConfig.Url, mailboxConfig.Port, SecureSocketOptions.SslOnConnect);
				await client.AuthenticateAsync(mailboxConfig.UserName, mailboxConfig.Password);

				var junkFolder = await GetFolderAsync(client);
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

				client.Disconnect(true);
			}

			await messagesHandler.CatalogMessagesAsync(messages);
		}

		public virtual Task DetectSpamAsync() => throw new NotImplementedException();

        protected virtual Task<IMailFolder> GetFolderAsync(ImapClient imapClient) => throw new NotImplementedException();
    }
}
