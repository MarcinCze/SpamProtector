using ProtectorLib.Models;

using System.Threading.Tasks;

namespace MessageEmailHandlerService
{
    public interface IEmailMessageHandler
    {
        Task HandleAsync(EmailDTO message);
    }
}
