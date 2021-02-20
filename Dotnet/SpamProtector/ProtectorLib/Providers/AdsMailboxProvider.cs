using Microsoft.Extensions.Logging;

using ProtectorLib.Configuration;
using ProtectorLib.Handlers;
using ProtectorLib.Messaging;

namespace ProtectorLib.Providers
{
    public class AdsMailboxProvider : BaseMailboxProvider
    {
        private readonly MailboxesConfig mailboxesConfig;

        public AdsMailboxProvider(
            MailboxesConfig mailboxesConfig,
            ServicesConfig servicesConfig,
            IMessagesHandler messagesHandler,
            IDateTimeProvider dateTimeProvider,
            IMessagingService messagingService,
            ILogger<MainMailboxProvider> logger)
            : base(servicesConfig, messagesHandler, dateTimeProvider, messagingService, logger)
        {
            this.mailboxesConfig = mailboxesConfig;
        }

        public override string MailBoxName => "REKLAMY";

        protected override MailboxConfig MailboxConfig => mailboxesConfig.AdsBox;
    }
}
