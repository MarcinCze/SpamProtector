using Microsoft.Extensions.DependencyInjection;

using ProtectorLib.Providers;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProtectorLib.Handlers
{
    public class MessagesHandler : IMessagesHandler
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IDateTimeProvider dateTimeProvider;
        private SpamProtectorDBContext dbContext;

        public MessagesHandler(IServiceScopeFactory serviceScopeFactory, IDateTimeProvider dateTimeProvider)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.dateTimeProvider = dateTimeProvider;
        }

        public async Task CatalogMessagesAsync(IEnumerable<Message> messages)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                foreach (var msg in messages.Where(m => !MessageExsists(m)))
                {
                    msg.CatalogTime = dateTimeProvider.CurrentTime;
                    await dbContext.Messages.AddAsync(msg);
                }

                await dbContext.SaveChangesAsync();
            }

            dbContext = null;
        }

        public async Task MarkForRemovalAsync()
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                foreach (var msg in dbContext.Messages.Where(x => !x.IsRemoved && x.CatalogTime != null && x.PlannedRemoveTime == null && x.RemoveTime == null))
                {
                    msg.PlannedRemoveTime = msg.CatalogTime.Value.AddDays(3).Date;
                }

                await dbContext.SaveChangesAsync();
            }

            dbContext = null;
        }

        public async Task<IEnumerable<Message>> GetMessagesForRemovalAsync(string mailbox)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                return await dbContext.Messages
                    .Where(x => x.Mailbox == mailbox)
                    .Where(x => !x.IsRemoved && x.PlannedRemoveTime <= dateTimeProvider.CurrentTime && x.RemoveTime == null)
                    .OrderBy(x => x.PlannedRemoveTime)
                    .Take(20)
                    .ToListAsync();
            }
        }

        public async Task MarkMessagesAsRemovedAsync(IEnumerable<int> removedMsgIds)
        {
            if (!removedMsgIds.Any())
                return;

            using (var scope = serviceScopeFactory.CreateScope())
            {
                dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                foreach (var msgId in removedMsgIds)
                {
                    var dbMsg = await dbContext.Messages.FirstOrDefaultAsync(x => x.Id == msgId);

                    if (dbMsg == null)
                        continue;

                    dbMsg.RemoveTime = dateTimeProvider.CurrentTime;
                }

                await dbContext.SaveChangesAsync();
            }
        }

        private bool MessageExsists(Message message) => dbContext.Messages.Any(x => x.ImapUid == message.ImapUid && x.Sender == message.Sender);
    }
}
