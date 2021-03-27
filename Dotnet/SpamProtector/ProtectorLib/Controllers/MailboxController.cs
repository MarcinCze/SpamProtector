using ProtectorLib.Providers;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtectorLib.Controllers
{
    public class MailboxController : IMailboxController
    {
        private readonly IEnumerable<IMailboxProvider> mailboxProviders;
        private int providerIndex;

        public MailboxController(IEnumerable<IMailboxProvider> mailboxProviders)
        {
            if (!mailboxProviders.Any())
                throw new ArgumentOutOfRangeException($"{nameof(mailboxProviders)} cannot be empty");

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
