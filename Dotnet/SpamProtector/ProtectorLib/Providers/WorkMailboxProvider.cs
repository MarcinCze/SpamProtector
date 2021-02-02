using ProtectorLib.Configuration;
using ProtectorLib.Handlers;

namespace ProtectorLib.Providers
{
    public class WorkMailboxProvider : BaseMailboxProvider
    {
        private readonly MailboxesConfig mailboxesConfig;

        public WorkMailboxProvider(
            MailboxesConfig mailboxesConfig,
            ServicesConfig servicesConfig,
            IMessagesHandler messagesHandler,
            IDateTimeProvider dateTimeProvider)
            : base(servicesConfig, messagesHandler, dateTimeProvider)
        {
            this.mailboxesConfig = mailboxesConfig;
        }

        public override string MailBoxName => "WORK";

        protected override MailboxConfig MailboxConfig => mailboxesConfig.WorkBox;
    }
}
