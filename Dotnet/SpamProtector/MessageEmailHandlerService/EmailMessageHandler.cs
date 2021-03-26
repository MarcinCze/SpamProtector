using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using ProtectorLib.Data;
using ProtectorLib.Models;

using System;
using System.Linq;
using System.Threading.Tasks;

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

                // Looking for message by given db id
                if (message.Id != 0)
                {
                    dbMessage = await dbContext.Messages.FirstAsync(x => x.Id == message.Id);

                    if (!ShouldBeUpdated(message, dbMessage))
                        return;

                    CopyValues(message, ref dbMessage);
                    await dbContext.SaveChangesAsync();
                    return;
                }
                
                // Looking for similar message already in database
                if (await dbContext.Messages.AnyAsync(x => 
                    x.ImapUid == message.ImapUid 
                    && x.Subject == message.Subject 
                    && x.Sender == message.Sender 
                    && x.Mailbox == message.Mailbox
                    && x.ReceivedTime == message.ReceivedTime))
                {
                    var mails = dbContext.Messages
                        .Where(x => x.ImapUid == message.ImapUid 
                            && x.Subject == message.Subject 
                            && x.Sender == message.Sender 
                            && x.Mailbox == message.Mailbox 
                            && x.ReceivedTime == message.ReceivedTime);

                    if (await mails.CountAsync() == 1)
                    {
                        dbMessage = await mails.FirstAsync();

                        if (!ShouldBeUpdated(message, dbMessage))
                            return;

                        CopyValues(message, ref dbMessage);
                        await dbContext.SaveChangesAsync();
                        return;
                    }

                    if (await mails.CountAsync() > 1)
                        throw new Exception("More than one message with given params");
                }

                // New message
                dbMessage = new Message();
                CopyValues(message, ref dbMessage);
                await dbContext.AddAsync(dbMessage);
                await dbContext.SaveChangesAsync();
                return;
            }
        }

        private bool ShouldBeUpdated(EmailDTO queueMessage, Message dbMessage)
        {
            if (dbMessage.VersionUpdateTime >= queueMessage.VersionUpdateTime)
                return false;

            return !(queueMessage.CatalogTime == dbMessage.CatalogTime 
                && queueMessage.Content == dbMessage.Content
                && queueMessage.ImapUid == dbMessage.ImapUid
                && queueMessage.IsRemoved == dbMessage.IsRemoved
                && queueMessage.Mailbox == dbMessage.Mailbox
                && queueMessage.PlannedRemoveTime == dbMessage.PlannedRemoveTime
                && queueMessage.ReceivedTime == dbMessage.ReceivedTime
                && queueMessage.Recipient == dbMessage.Recipient
                && queueMessage.RemoveTime == dbMessage.RemoveTime
                && queueMessage.Sender == dbMessage.Sender
                && queueMessage.Subject == dbMessage.Subject);
        }

        private void CopyValues(EmailDTO queueMessage, ref Message dbMessage)
        {
            dbMessage.CatalogTime = queueMessage.CatalogTime;
            dbMessage.ImapUid = queueMessage.ImapUid;
            dbMessage.Content = queueMessage.Content;
            dbMessage.IsRemoved = queueMessage.IsRemoved;
            dbMessage.Mailbox = queueMessage.Mailbox;
            dbMessage.PlannedRemoveTime = queueMessage.PlannedRemoveTime;
            dbMessage.ReceivedTime = queueMessage.ReceivedTime;
            dbMessage.Recipient = queueMessage.Recipient;
            dbMessage.RemoveTime = queueMessage.RemoveTime;
            dbMessage.Sender = queueMessage.Sender;
            dbMessage.Subject = queueMessage.Subject;
            dbMessage.VersionUpdateTime = queueMessage.VersionUpdateTime;
        }
    }
}
