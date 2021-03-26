using System.Threading.Tasks;

namespace MessageLogHandlerService
{
    public interface IMessageLogHandler
    {
        Task HandleAsync(ProtectorLib.Models.LogEntryDTO message);
    }
}
