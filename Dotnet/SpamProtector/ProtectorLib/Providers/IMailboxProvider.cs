using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public interface IMailboxProvider
    {
        Task CatalogAsync();
    }
}
