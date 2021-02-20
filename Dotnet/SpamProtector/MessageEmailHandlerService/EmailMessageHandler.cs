using ProtectorLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using ProtectorLib;

namespace MessageEmailHandlerService
{
    public class EmailMessageHandler : IEmailMessageHandler
    {
        IServiceScopeFactory serviceScopeFactory;

        public EmailMessageHandler(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task HandleAsync(EmailDTO message)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                Message dbMessage = null;

                if (message.Id != 0)
                {
                    dbMessage = await dbContext.Messages.FirstAsync(x => x.Id == message.Id);

                    if (dbMessage.VersionUpdateTime >= message.VersionUpdateTime)
                        return;

                    dbMessage.CatalogTime = message.CatalogTime;
                    dbMessage.ImapUid = message.ImapUid;
                    dbMessage.Content = message.Content;
                    dbMessage.IsRemoved = message.IsRemoved;
                    dbMessage.Mailbox = message.Mailbox;
                    dbMessage.PlannedRemoveTime = message.PlannedRemoveTime;
                    dbMessage.ReceivedTime = message.ReceivedTime;
                    dbMessage.Recipient = message.Recipient;
                    dbMessage.RemoveTime = message.RemoveTime;
                    dbMessage.Sender = message.Sender;
                    dbMessage.Subject = message.Subject;
                    dbMessage.VersionUpdateTime = message.VersionUpdateTime;
                }
                else
                {
                    await dbContext.Messages.AddAsync(new Message
                    {
                        CatalogTime = message.CatalogTime,
                        Content = message.Content,
                        ImapUid = message.ImapUid,
                        IsRemoved = message.IsRemoved,
                        Mailbox = message.Mailbox,
                        PlannedRemoveTime = message.PlannedRemoveTime,
                        ReceivedTime = message.ReceivedTime,
                        Recipient = message.Recipient,
                        RemoveTime = message.RemoveTime,
                        Sender = message.Sender,
                        Subject = message.Sender,
                        VersionUpdateTime = message.VersionUpdateTime
                    });
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
