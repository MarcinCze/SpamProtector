using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using ProtectorLib.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtectorLib.Handlers
{
    public class MessagesHandler : IMessagesHandler
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ILogger<MessagesHandler> logger;
        private SpamProtectorDBContext dbContext;

        public MessagesHandler(IServiceScopeFactory serviceScopeFactory, IDateTimeProvider dateTimeProvider, ILogger<MessagesHandler> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.dateTimeProvider = dateTimeProvider;
            this.logger = logger;
        }

        public async Task<int> CatalogMessagesAsync(IEnumerable<Message> messages)
        {
            int msgInserted = 0;
            using (var scope = serviceScopeFactory.CreateScope())
            {
                dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                foreach (var msg in messages.Where(m => !MessageExists(m)))
                {
                    msg.CatalogTime = dateTimeProvider.CurrentTime;
                    await dbContext.Messages.AddAsync(msg);
                    msgInserted++;
                }

                await dbContext.SaveChangesAsync();
            }

            dbContext = null;
            return msgInserted;
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

        public async Task<IEnumerable<Message>> GetRemovedMessagesForCheckingAsync()
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                if (!await dbContext.Messages.AnyAsync(x => x.RemoveTime != null && !x.IsRemoved))
                {
                    logger.LogWarning($"{nameof(GetRemovedMessagesForCheckingAsync)}: there are no messages where RemoveTime is not null and IsRemoved = false");
                    return new List<Message>();
                }

                return await dbContext.Messages
                    .Where(x => x.RemoveTime != null && !x.IsRemoved)
                    .OrderBy(x => x.CatalogTime)
                    .Take(5)
                    .ToListAsync();
            }
        }

        public async Task SetMessagesAsPermamentlyRemovedAsync(IEnumerable<int> removedMsgsIds)
        {
            if (!removedMsgsIds.Any())
            {
                logger.LogWarning($"{nameof(SetMessagesAsPermamentlyRemovedAsync)}: provided checked messages list is empty");
                return;
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                foreach (var checkedMsgId in removedMsgsIds)
                {
                    var msg = await dbContext.Messages.FirstOrDefaultAsync(x => x.Id == checkedMsgId);

                    if (msg == null)
                    {
                        logger.LogWarning($"{nameof(SetMessagesAsPermamentlyRemovedAsync)}: checked message with id {checkedMsgId} doesn't exist in database");
                        continue;
                    }

                    msg.IsRemoved = true;
                }

                await dbContext.SaveChangesAsync();
            }
        }

        private bool MessageExists(Message message) => dbContext.Messages.Any(x => x.ImapUid == message.ImapUid && x.Sender == message.Sender);
    }
}
