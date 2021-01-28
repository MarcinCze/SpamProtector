using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public interface IMailboxProvider
    {
        Task CatalogAsync();
        Task DetectSpamAsync();
        Task<(int countBefore, int countAfter)> DeleteMessagesAsync();
    }
}
