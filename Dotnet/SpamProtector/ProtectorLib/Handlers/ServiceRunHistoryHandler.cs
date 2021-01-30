using Microsoft.Extensions.DependencyInjection;

using ProtectorLib.Providers;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProtectorLib.Handlers
{
    public class ServiceRunHistoryHandler : IServiceRunHistoryHandler
    {
        public enum ServiceStatus
        {
            PROCESSING,
            ERROR,
            DONE
        }

        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IDateTimeProvider dateTimeProvider;
        private int entryId;

        public ServiceRunHistoryHandler(IServiceScopeFactory serviceScopeFactory, IDateTimeProvider dateTimeProvider)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.dateTimeProvider = dateTimeProvider;
        }

        public async Task RegisterStartAsync(string serviceName, string serviceVersion)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                var entry = new ServiceRunHistory
                {
                    ServiceName = serviceName,
                    ServiceVersion = GetVersionEntry(serviceVersion),
                    Status = ServiceStatus.PROCESSING.ToString(),
                    StartTime = dateTimeProvider.CurrentTime
                };
                await dbContext.ServiceRunHistories.AddAsync(entry);

                await dbContext.SaveChangesAsync();
                entryId = entry.Id;
            }
        }

        public async Task RegisterFinishAsync(string serviceName, string additionalData, ServiceStatus endStatus, string executionTime)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                var entry = dbContext.ServiceRunHistories
                    .OrderByDescending(x => x.StartTime)
                    .FirstOrDefault(x => x.Id == entryId);

                if (entry == null)
                    return;

                entry.Status = endStatus.ToString();
                entry.EndTime = dateTimeProvider.CurrentTime;
                entry.Information = additionalData;
                entry.ExecutionTime = executionTime;

                await dbContext.SaveChangesAsync();
            }
        }

        private string GetVersionEntry(string serviceVersion) => $"ver {serviceVersion} / lib {GetType().Assembly.GetName().Version}";
    }
}
