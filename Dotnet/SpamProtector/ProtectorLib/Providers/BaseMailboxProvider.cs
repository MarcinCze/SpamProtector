using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public abstract class BaseMailboxProvider : IMailboxProvider
    {
        public BaseMailboxProvider()
        {

        }

        public abstract Task CatalogAsync();
    }
}
