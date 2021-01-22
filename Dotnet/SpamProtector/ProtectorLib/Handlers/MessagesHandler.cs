using Microsoft.Extensions.DependencyInjection;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtectorLib.Handlers
{
    public class MessagesHandler : IMessagesHandler
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private SpamProtectorDBContext dbContext;

        public MessagesHandler(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task CatalogMessagesAsync(IEnumerable<Message> messages)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                foreach (var msg in messages.Where(m => !MessageExsists(m)))
                    await dbContext.Messages.AddAsync(msg);

                await dbContext.SaveChangesAsync();
            }

            dbContext = null;
        }

        private bool MessageExsists(Message message) => dbContext.Messages.Any(x => x.ImapUid == message.ImapUid && x.Sender == message.Sender);
    }
}
