using ProtectorLib.Configuration;
using ProtectorLib.Handlers;

namespace ProtectorLib.Providers
{
    public class AdsMailboxProvider : BaseMailboxProvider
    {
        private readonly MailboxesConfig mailboxesConfig;

        public AdsMailboxProvider(
            MailboxesConfig mailboxesConfig,
            ServicesConfig servicesConfig,
            IMessagesHandler messagesHandler,
            IDateTimeProvider dateTimeProvider)
            : base(servicesConfig, messagesHandler, dateTimeProvider)
        {
            this.mailboxesConfig = mailboxesConfig;
        }

        public override string MailBoxName => "REKLAMY";

        protected override MailboxConfig MailboxConfig => mailboxesConfig.AdsBox;
    }
}
