using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;

using Microsoft.Extensions.Logging;

using ProtectorLib.Configuration;
using ProtectorLib.Data;
using ProtectorLib.Extensions;
using ProtectorLib.Handlers;
using ProtectorLib.Messaging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public abstract class BaseMailboxProvider : IMailboxProvider
    {
        protected readonly ServicesConfig servicesConfig;
        protected readonly IMessagesHandler messagesHandler;
		protected readonly IDateTimeProvider dateTimeProvider;
		protected readonly ILogger logger;
		protected readonly IMessagingService messagingService;

        protected BaseMailboxProvider(
            ServicesConfig servicesConfig, 
			IMessagesHandler messagesHandler, 
			IDateTimeProvider dateTimeProvider,
			IMessagingService messagingService,
			ILogger logger)
        {
			this.logger = logger;
			this.messagingService = messagingService;
            this.servicesConfig = servicesConfig;
            this.messagesHandler = messagesHandler;
			this.dateTimeProvider = dateTimeProvider;
        }

		public abstract string MailBoxName { get; }
		protected abstract MailboxConfig MailboxConfig { get; }

		protected virtual DateTime DeliveredAfterDate => dateTimeProvider.CurrentTime.Date.AddDays(-servicesConfig.CatalogDaysToCheck);
		protected virtual DateTime DeliveredAfterDateScan => dateTimeProvider.CurrentTime.Date.AddDays(-servicesConfig.ScanDaysToCheck);

		public virtual async Task<int> CatalogAsync()
        {
			using (var client = new ImapClient())
			{
				await client.ConnectAsync(MailboxConfig.Url, MailboxConfig.Port, SecureSocketOptions.SslOnConnect);
				await client.AuthenticateAsync(MailboxConfig.UserName, MailboxConfig.Password);
				logger.LogInformation("Client connected & authorized");

				var junkFolder = await GetJunkFolderAsync(client);
				await junkFolder.OpenAsync(FolderAccess.ReadOnly);
				logger.LogInformation($"Junk folder {junkFolder.FullName} opened");
				var uids = await junkFolder.SearchAsync(SearchQuery.DeliveredAfter(DeliveredAfterDate));

				var mails = new List<Models.EmailDTO>();
				foreach (var uid in uids)
				{
					var message = await junkFolder.GetMessageAsync(uid);
					mails.Add(new Models.EmailDTO
					{
						ImapUid = (int)uid.Id,
						Mailbox = MailBoxName,
						Recipient = message.To.Mailboxes.FirstOrDefault()?.Address,
						Sender = message.From.Mailboxes.FirstOrDefault()?.Address,
						Subject = message.Subject,
						Content = string.IsNullOrEmpty(message.TextBody) ? "TEXT BODY NOT PROVIDED. ONLY HTML" : message.TextBody,
						ReceivedTime = message.Date.DateTime,
						CatalogTime = dateTimeProvider.CurrentTime,
						VersionUpdateTime = dateTimeProvider.CurrentTime
					});
				}
				messagingService.SendMessages(mails);

				await client.DisconnectAsync(true);
				logger.LogInformation("Client disconnected");

				return mails.Count;
			}
		}

		public virtual async Task<(int countBefore, int countAfter)> DeleteMessagesAsync()
        {
			var messagesToRemove = await messagesHandler.GetMessagesForRemovalAsync(MailBoxName);
			var messagesRemoved = new List<Message>();

			using (var client = new ImapClient())
			{
				await client.ConnectAsync(MailboxConfig.Url, MailboxConfig.Port, SecureSocketOptions.SslOnConnect);
				await client.AuthenticateAsync(MailboxConfig.UserName, MailboxConfig.Password);
				logger.LogInformation("Client connected & authorized");

				var junkFolder = await GetJunkFolderAsync(client);
				await junkFolder.OpenAsync(FolderAccess.ReadWrite);
                logger.LogInformation($"Junk folder {junkFolder.FullName} opened");
				int countBefore = junkFolder.Count;

				if (!messagesToRemove.Any())
                {
					logger.LogWarning("There are no messages to remove");
					await DeleteConfirmationProcessAsync(client, junkFolder);
					await client.DisconnectAsync(true);
					return (countBefore, countBefore);
				}

				foreach (var message in messagesToRemove)
                {
					try
                    {
						var uid = new UniqueId((uint)message.ImapUid);
						var mail = await junkFolder.GetMessageAsync(uid);
						await junkFolder.AddFlagsAsync(uid, MessageFlags.Deleted, true);
						message.RemoveTime = dateTimeProvider.CurrentTime;
						messagesRemoved.Add(message);
					}
                    catch (Exception ex)
                    {
						logger.LogError(ex, ex.Message);
					}
                }

				await junkFolder.ExpungeAsync();
				logger.LogInformation("Junk folder expunged");
				int countAfter = junkFolder.Count;

				messagingService.SendMessages(messagesRemoved.ConvertToDto(dateTimeProvider.CurrentTime));

				await DeleteConfirmationProcessAsync(client, junkFolder);
                logger.LogInformation("DeleteConfirmationProcess done");

				await client.DisconnectAsync(true);
				logger.LogInformation("Client disconnected");

				return (countBefore, countAfter);
			}
		}

		public virtual Task<int> DetectSpamAsync() => throw new NotImplementedException();

        protected virtual Task<IMailFolder> GetJunkFolderAsync(ImapClient imapClient) => throw new NotImplementedException();

		protected virtual async Task DeleteConfirmationProcessAsync(ImapClient imapClient, IMailFolder junkFolder)
        {
			List<Message> messagesForChecking = (await messagesHandler.GetRemovedMessagesForCheckingAsync())?.ToList();

			if (!messagesForChecking.Any())
				return;

			var uids = await junkFolder.SearchAsync(SearchQuery.Uids(messagesForChecking.GetUniqueIds()));

			if (!uids.Any())
            {
				logger.LogInformation($"{nameof(DeleteConfirmationProcessAsync)}: all messages doesn't exists which is OK");
				messagesForChecking.ForEach(msg => msg.IsRemoved = true);
				messagingService.SendMessages(messagesForChecking.ConvertToDto(dateTimeProvider.CurrentTime));
				return;
            }

            foreach (var message in uids)
            {
				logger.LogError($"Message with ID {message.Id} should be removed but exists");
				messagesForChecking.RemoveAll(x => x.ImapUid == message.Id);
            }

			messagesForChecking.ForEach(msg => msg.IsRemoved = true);
			messagingService.SendMessages(messagesForChecking.ConvertToDto(dateTimeProvider.CurrentTime));

			logger.LogInformation($"{messagesForChecking.Count} are set as PERMAMENTLY removed");
		}
	}
}
