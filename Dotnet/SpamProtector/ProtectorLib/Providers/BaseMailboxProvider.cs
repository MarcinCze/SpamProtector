using Microsoft.Extensions.Logging;

using ProtectorLib.Configuration;
using ProtectorLib.Handlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public abstract class BaseMailboxProvider : IMailboxProvider
    {
        protected readonly MailboxConfig mailboxConfig;
        protected readonly ServicesConfig servicesConfig;
        protected readonly IMessagesHandler messagesHandler;

        public BaseMailboxProvider(MailboxConfig mailboxConfig, ServicesConfig servicesConfig, IMessagesHandler messagesHandler)
        {
            this.mailboxConfig = mailboxConfig;
            this.servicesConfig = servicesConfig;
            this.messagesHandler = messagesHandler;
        }

        public abstract Task CatalogAsync();

        protected DateTime DeliveredAfterDate => DateTime.Now.Date.AddDays(-servicesConfig.CatalogDaysToCheck);
    }
}
