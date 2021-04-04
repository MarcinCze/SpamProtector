using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;

using Microsoft.Extensions.Logging;

using MimeKit;

using ProtectorLib.Configuration;
using ProtectorLib.Handlers;
using ProtectorLib.Messaging;

using System.Linq;
using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public class AdsMailboxProvider : BaseMailboxProvider
    {
        private readonly MailboxesConfig mailboxesConfig;
        private readonly IRulesProvider rulesProvider;

        public AdsMailboxProvider(
            MailboxesConfig mailboxesConfig,
            ServicesConfig servicesConfig,
            IMessagesHandler messagesHandler,
            IDateTimeProvider dateTimeProvider,
            IMessagingService messagingService,
            IRulesProvider rulesProvider,
            ILogger<MainMailboxProvider> logger)
            : base(servicesConfig, messagesHandler, dateTimeProvider, messagingService, logger)
        {
            this.mailboxesConfig = mailboxesConfig;
            this.rulesProvider = rulesProvider;
        }

        public override string MailBoxName => "REKLAMY";

        protected override MailboxConfig MailboxConfig => mailboxesConfig.AdsBox;

        public async override Task<int> DetectSpamAsync()
        {
            int newSpamCounter = 0;
            using (var client = new ImapClient())
            {
                await client.ConnectAsync(MailboxConfig.Url, MailboxConfig.Port, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(MailboxConfig.UserName, MailboxConfig.Password);
                logger.LogInformation("Client connected & authorized");
                await client.Inbox.OpenAsync(FolderAccess.ReadWrite);
                var destinationFolder = await GetJunkFolderAsync(client);

                var uids = await client.Inbox.SearchAsync(SearchQuery.DeliveredAfter(DeliveredAfterDateScan));

                foreach (var uid in uids)
                {
                    var message = await client.Inbox.GetMessageAsync(uid);

                    if (await IsSpam(message))
                    {
                        logger.LogInformation($"Message with UID {uid} is a SPAM");
                        newSpamCounter++;
                        await client.Inbox.MoveToAsync(uid, destinationFolder);
                    }
                }

                await client.DisconnectAsync(true);
                logger.LogInformation("Client disconnected");
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

        protected override async Task<IMailFolder> GetJunkFolderAsync(ImapClient imapClient) => await imapClient.Inbox.GetSubfolderAsync("Spambox");
    }
}
