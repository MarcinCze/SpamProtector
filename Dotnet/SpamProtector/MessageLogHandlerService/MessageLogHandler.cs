using Microsoft.Extensions.DependencyInjection;

using ProtectorLib.Data;
using ProtectorLib.Models;

using System.Threading.Tasks;

namespace MessageLogHandlerService
{
    public class MessageLogHandler : IMessageLogHandler
    {
        IServiceScopeFactory serviceScopeFactory;

        public MessageLogHandler(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task HandleAsync(LogEntryDTO message)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                await dbContext.Logs.AddAsync(new Log
                {
                    Type = message.Type,
                    ServiceName = message.ServiceName,
                    Branch = message.Branch,
                    Message = message.Message,
                    StackTrace = message.StackTrace,
                    ServiceVersion = message.ServiceVersion,
                    Function = message.Function,
                    CreationTime = message.CreationTime,
                    AdditionalData = message.AdditionalData
                });

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
