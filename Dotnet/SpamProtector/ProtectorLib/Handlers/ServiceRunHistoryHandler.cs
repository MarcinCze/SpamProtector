using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using ProtectorLib.Providers;

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

        private readonly ILogger<ServiceRunHistoryHandler> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IDateTimeProvider dateTimeProvider;
        private int entryId;

        public ServiceRunHistoryHandler(
            IServiceScopeFactory serviceScopeFactory, 
            IDateTimeProvider dateTimeProvider, 
            ILogger<ServiceRunHistoryHandler> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.dateTimeProvider = dateTimeProvider;
            this.logger = logger;
        }

        public async Task RegisterStartAsync(string serviceName, string serviceVersion) =>
            await RegisterStartAsync(serviceName, serviceVersion, null);

        public async Task RegisterStartAsync(string serviceName, string serviceVersion, string branchName)
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                    var entry = new ServiceRunHistory
                    {
                        ServiceName = serviceName,
                        ServiceVersion = GetVersionEntry(serviceVersion),
                        Branch = branchName,
                        Status = ServiceStatus.PROCESSING.ToString(),
                        StartTime = dateTimeProvider.CurrentTime
                    };

                    await dbContext.ServiceRunHistories.AddAsync(entry);
                    await dbContext.SaveChangesAsync();
                    entryId = entry.Id;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        public async Task RegisterFinishAsync(string serviceName, string additionalData, ServiceStatus endStatus, string executionTime) =>
            await RegisterFinishAsync(serviceName, null, additionalData, endStatus, executionTime);

        public async Task RegisterFinishAsync(string serviceName, string branchName, string additionalData, ServiceStatus endStatus, string executionTime)
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                    var entry = dbContext.ServiceRunHistories
                        .OrderByDescending(x => x.StartTime)
                        .FirstOrDefault(x => x.Id == entryId && x.ServiceName.Equals(serviceName) && x.Branch.Equals(branchName));

                    if (entry == null)
                        return;

                    entry.Status = endStatus.ToString();
                    entry.EndTime = dateTimeProvider.CurrentTime;
                    entry.Information = additionalData.Length < 999 ? additionalData : additionalData.Substring(0, 999);
                    entry.ExecutionTime = executionTime;

                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        private string GetVersionEntry(string serviceVersion) => $"ver {serviceVersion} / lib {GetType().Assembly.GetName().Version}";
    }
}
