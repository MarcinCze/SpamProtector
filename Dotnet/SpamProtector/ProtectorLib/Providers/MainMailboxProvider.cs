﻿using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;

using MimeKit;

using ProtectorLib.Configuration;
using ProtectorLib.Handlers;

using System.Linq;
using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public class MainMailboxProvider : BaseMailboxProvider
    {
        private readonly MailboxesConfig mailboxesConfig;
        private readonly IRulesProvider rulesProvider;

		public MainMailboxProvider(
            MailboxesConfig mailboxesConfig,
            ServicesConfig servicesConfig, 
            IMessagesHandler messagesHandler,
            IRulesProvider rulesProvider,
			IDateTimeProvider dateTimeProvider) 
			: base(servicesConfig, messagesHandler, dateTimeProvider)
        {
            this.mailboxesConfig = mailboxesConfig;
            this.rulesProvider = rulesProvider;
		}

        public override string MailBoxName => "MARCIN";

        protected override MailboxConfig MailboxConfig => mailboxesConfig.MainBox;

        public override async Task<int> DetectSpamAsync()
        {
            int newSpamCounter = 0;
			using (var client = new ImapClient())
			{
				await client.ConnectAsync(MailboxConfig.Url, MailboxConfig.Port, SecureSocketOptions.SslOnConnect);
				await client.AuthenticateAsync(MailboxConfig.UserName, MailboxConfig.Password);
				await client.Inbox.OpenAsync(FolderAccess.ReadWrite);
				var destinationFolder = await client.Inbox.GetSubfolderAsync("Junk");

				var uids = await client.Inbox.SearchAsync(SearchQuery.DeliveredAfter(DeliveredAfterDateScan));

				foreach (var uid in uids)
				{
					var message = await client.Inbox.GetMessageAsync(uid);

                    if (await IsSpam(message))
                    {
                        newSpamCounter++;
                        await client.Inbox.MoveToAsync(uid, destinationFolder);
					}
                }

				await client.DisconnectAsync(true);
			}

            return newSpamCounter;
        }

		protected async Task<bool> IsSpam(MimeMessage message)
        {
			string sender = message.From.Mailboxes.FirstOrDefault()?.Address;
			if (await rulesProvider.IsInSenderBlacklist(sender))
				return true;

            string domain = sender?.Substring(sender.IndexOf('@') + 1);
            if (await rulesProvider.IsInDomainBlacklist(domain))
                return true;

            if (await rulesProvider.IsInSubjectBlacklist(message.Subject))
                return true;

            return false;
        }

        protected override async Task<IMailFolder> GetJunkFolderAsync(ImapClient imapClient) => await imapClient.Inbox.GetSubfolderAsync("Junk");
    }
}
