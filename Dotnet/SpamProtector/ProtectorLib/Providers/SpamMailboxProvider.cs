using MailKit;
using MailKit.Net.Imap;

using ProtectorLib.Configuration;
using ProtectorLib.Handlers;

using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public class SpamMailboxProvider : BaseMailboxProvider
    {
        private readonly MailboxesConfig mailboxesConfig;

        public SpamMailboxProvider(
            MailboxesConfig mailboxesConfig, 
            ServicesConfig servicesConfig, 
            IMessagesHandler messagesHandler,
            IDateTimeProvider dateTimeProvider) 
            : base(servicesConfig, messagesHandler, dateTimeProvider)
        {
            this.mailboxesConfig = mailboxesConfig;
        }

        public override string MailBoxName => "SPAM";

        protected override MailboxConfig MailboxConfig => mailboxesConfig.SpamBox;

        protected override async Task<IMailFolder> GetJunkFolderAsync(ImapClient imapClient) => await imapClient.GetFolderAsync("Inbox");
    }
}
