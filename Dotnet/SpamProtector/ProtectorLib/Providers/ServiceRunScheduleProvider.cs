using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using ProtectorLib.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public class ServiceRunScheduleProvider : IServiceRunScheduleProvider
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IDateTimeProvider dateTimeProvider;
        private List<ServiceRunSchedule> cachedServices;

        public ServiceRunScheduleProvider(IServiceScopeFactory serviceScopeFactory, IDateTimeProvider dateTimeProvider)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.dateTimeProvider = dateTimeProvider;
            this.cachedServices = new List<ServiceRunSchedule>();
        }

        public async Task<bool> ShouldRunAsync(string serviceName) => await ShouldRunAsync(serviceName, null);

        public async Task<bool> ShouldRunAsync(string serviceName, string branchName)
        {
            if (GetCachedService(serviceName, branchName) == null)
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();
                    ServiceRunSchedule service = await dbContext.ServiceRunSchedules.FirstOrDefaultAsync(x => x.ServiceName == serviceName && x.Branch.Equals(branchName) && x.IsEnabled);

                    cachedServices.Add(service ?? new ServiceRunSchedule
                    {
                        ServiceName = serviceName,
                        Branch = branchName,
                        IsEnabled = false
                    });
                }
            }

            if (!GetCachedService(serviceName, branchName).IsEnabled)
                return false;

            var nextRun = CalculateNextRun(GetCachedService(serviceName, branchName));
            return nextRun <= dateTimeProvider.CurrentTime;
        }

        public async Task SaveLastRunAsync(string serviceName) => await SaveLastRunAsync(serviceName, null);

        public async Task SaveLastRunAsync(string serviceName, string branchName)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                var serviceEntry = await dbContext.ServiceRunSchedules.FirstOrDefaultAsync(x => x.ServiceName.Equals(serviceName) && x.Branch.Equals(branchName));

                if (serviceName == null)
                    return;

                serviceEntry.LastRun = dateTimeProvider.CurrentTime;
                await dbContext.SaveChangesAsync();
                cachedServices = new List<ServiceRunSchedule>();
            }
        }

        protected DateTime CalculateNextRun(ServiceRunSchedule service)
        {
            if (!service.LastRun.HasValue)
                return dateTimeProvider.CurrentTime;

            return service.LastRun.Value
                .AddDays(service.RunEveryDays)
                .AddHours(service.RunEveryHours)
                .AddMinutes(service.RunEveryMinutes)
                .AddSeconds(service.RunEverySeconds);
        }

        protected ServiceRunSchedule GetCachedService(string serviceName, string branchName) =>
            cachedServices.FirstOrDefault(x => x.ServiceName.Equals(serviceName) && x.Branch == branchName);
    }
}
