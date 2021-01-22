using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public class SpamMailboxProvider : BaseMailboxProvider
    {
        public SpamMailboxProvider()
        {

        }

        public override Task CatalogAsync()
        {
            throw new NotImplementedException();
        }
    }
}
