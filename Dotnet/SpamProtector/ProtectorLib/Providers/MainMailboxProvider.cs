using MailKit;
using MailKit.Net.Imap;

using ProtectorLib.Configuration;
using ProtectorLib.Handlers;

using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public class MainMailboxProvider : BaseMailboxProvider
    {
		public MainMailboxProvider(MailboxConfig mailboxConfig, ServicesConfig servicesConfig, IMessagesHandler messagesHandler) 
			: base(mailboxConfig, servicesConfig, messagesHandler)
        {
			MailBoxName = "MARCIN";
		}

        protected async override Task<IMailFolder> GetFolderAsync(ImapClient imapClient) => await imapClient.Inbox.GetSubfolderAsync("Junk");
    }
}
