using ProtectorLib.Providers;

namespace ProtectorLib.Controllers
{
    public interface IMailboxController
    {
        IMailboxProvider CurrentMailboxProvider { get; }
        void SetNextProvider();
    }
}
