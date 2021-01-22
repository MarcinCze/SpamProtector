using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;

using System;
using System.Threading.Tasks;
using ProtectorLib.Configuration;

namespace ProtectorLib.Providers
{
    public class MainMailboxProvider : BaseMailboxProvider
    {
		private readonly MainBoxConfig mainBoxConfig;

		public MainMailboxProvider(MainBoxConfig mainBoxConfig)
        {
			this.mainBoxConfig = mainBoxConfig;
        }

		public override async Task CatalogAsync()
		{
            try
            {
				using (var client = new ImapClient())
				{
					await client.ConnectAsync(mainBoxConfig.Url, mainBoxConfig.Port, SecureSocketOptions.SslOnConnect);
					await client.AuthenticateAsync(mainBoxConfig.UserName, mainBoxConfig.Password);

					var junkFolder = await client.Inbox.GetSubfolderAsync("Junk");
					await junkFolder.OpenAsync(FolderAccess.ReadOnly);
					var uids = await junkFolder.SearchAsync(SearchQuery.DeliveredAfter(new DateTime(2021, 1, 19)));

					foreach (var uid in uids)
					{
						var message = await junkFolder.GetMessageAsync(uid);

						// write the message to a file
						//message.WriteTo(string.Format("{0}.eml", uid));
					}

					client.Disconnect(true);
				}
			}
            catch (Exception ex)
            {
                throw;
            }
			
		}
    }
}
