using MailKit;
using MailKit.Net.Imap;

using ProtectorLib.Configuration;
using ProtectorLib.Handlers;

using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public class SpamMailboxProvider : BaseMailboxProvider
    {
        public SpamMailboxProvider(MailboxConfig mailboxConfig, ServicesConfig servicesConfig, IMessagesHandler messagesHandler) 
            : base(mailboxConfig, servicesConfig, messagesHandler)
        {
            MailBoxName = "SPAM";
        }

        protected async override Task<IMailFolder> GetFolderAsync(ImapClient imapClient) => await imapClient.GetFolderAsync("Inbox");
    }
}
