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
    public class MainMailboxProvider : BaseMailboxProvider
    {
		public MainMailboxProvider(MailboxConfig mailboxConfig, ServicesConfig servicesConfig, IMessagesHandler messagesHandler) 
			: base(mailboxConfig, servicesConfig, messagesHandler)
        { }

		public override async Task CatalogAsync()
		{
			var messages = new List<Message>();

            try
            {
				using (var client = new ImapClient())
				{
					await client.ConnectAsync(mailboxConfig.Url, mailboxConfig.Port, SecureSocketOptions.SslOnConnect);
					await client.AuthenticateAsync(mailboxConfig.UserName, mailboxConfig.Password);

					var junkFolder = await client.Inbox.GetSubfolderAsync("Junk");
					await junkFolder.OpenAsync(FolderAccess.ReadOnly);
					var uids = await junkFolder.SearchAsync(SearchQuery.DeliveredAfter(DeliveredAfterDate));

					foreach (var uid in uids)
					{
						var message = await junkFolder.GetMessageAsync(uid);

						messages.Add(new Message
						{
							ImapUid = (int)uid.Id,
							Mailbox = "MARCIN",
							Recipient = message.To.FirstOrDefault()?.Name,
							Sender = message.From.FirstOrDefault()?.Name,
							Subject = message.Subject,
							Content = message.TextBody,
						});
					}

					client.Disconnect(true);
				}
			}
            catch (Exception ex)
            {
                throw;
            }

			await messagesHandler.CatalogMessagesAsync(messages);
		}
    }
}
