using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public interface IMailboxProvider
    {
        Task<int> CatalogAsync();
        Task<int> DetectSpamAsync();
        Task<(int countBefore, int countAfter)> DeleteMessagesAsync();
    }
}
