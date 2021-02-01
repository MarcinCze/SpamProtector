using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public interface IRulesProvider
    {
        Task<bool> IsInSenderBlacklist(string sender);
        Task<bool> IsInDomainBlacklist(string domain);
        Task<bool> IsInSubjectBlacklist(string subject);
    }
}
