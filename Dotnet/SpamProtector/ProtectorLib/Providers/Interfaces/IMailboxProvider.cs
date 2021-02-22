using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public interface IMailboxProvider
    {
        string MailBoxName { get; }
        Task CatalogAsync();
        Task<int> DetectSpamAsync();
        Task<(int countBefore, int countAfter)> DeleteMessagesAsync();
    }
}
