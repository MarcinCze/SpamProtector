using Microsoft.Extensions.Logging;

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
            IDateTimeProvider dateTimeProvider,
            ILogger<MainMailboxProvider> logger)
            : base(servicesConfig, messagesHandler, dateTimeProvider, logger)
        {
            this.mailboxesConfig = mailboxesConfig;
        }

        public override string MailBoxName => "WORK";

        protected override MailboxConfig MailboxConfig => mailboxesConfig.WorkBox;
    }
}
