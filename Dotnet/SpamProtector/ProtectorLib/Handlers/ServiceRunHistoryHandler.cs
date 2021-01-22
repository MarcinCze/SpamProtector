using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;
using System.Threading.Tasks;

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
        private int entryId;

        public ServiceRunHistoryHandler(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task RegisterStartAsync(string serviceName)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                var entry = new ServiceRunHistory
                {
                    ServiceName = serviceName,
                    Status = ServiceStatus.PROCESSING.ToString(),
                    StartTime = DateTime.Now
                };
                await dbContext.ServiceRunHistories.AddAsync(entry);

                var service = dbContext.ServiceRunSchedules.FirstOrDefault(x => x.ServiceName == serviceName);
                service.LastRun = DateTime.Now;

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
                    //.FirstOrDefault(x => x.ServiceName == serviceName && x.Status == ServiceStatus.PROCESSING.ToString());

                entry.Status = endStatus.ToString();
                entry.EndTime = DateTime.Now;
                entry.Information = additionalData;
                entry.ExecutionTime = executionTime;

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
