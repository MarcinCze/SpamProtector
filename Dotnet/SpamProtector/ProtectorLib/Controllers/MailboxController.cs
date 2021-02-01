using System.Collections.Generic;
using System.Linq;
using ProtectorLib.Providers;

namespace ProtectorLib.Controllers
{
    public class MailboxController : IMailboxController
    {
        private readonly IEnumerable<IMailboxProvider> mailboxProviders;
        private int providerIndex;

        public MailboxController(IEnumerable<IMailboxProvider> mailboxProviders)
        {
            this.mailboxProviders = mailboxProviders;
            providerIndex = 0;
        }

        public IMailboxProvider CurrentMailboxProvider => mailboxProviders.ElementAt(providerIndex);

        public void SetNextProvider()
        {
            providerIndex = (providerIndex + 1) >= mailboxProviders.Count() ? 0 : providerIndex + 1;
        }
    }
}
