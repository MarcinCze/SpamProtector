using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public class ServiceRunScheduleProvider : IServiceRunScheduleProvider
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IDateTimeProvider dateTimeProvider;
        private ServiceRunSchedule cachedServiceRunSchedule;

        public ServiceRunScheduleProvider(IServiceScopeFactory serviceScopeFactory, IDateTimeProvider dateTimeProvider)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.dateTimeProvider = dateTimeProvider;
        }

        public async Task<bool> ShouldRunAsync(string serviceName)
        {
            if (cachedServiceRunSchedule == null)
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();
                    cachedServiceRunSchedule = await dbContext.ServiceRunSchedules.FirstAsync(x => x.ServiceName == serviceName);
                }
            }

            var nextRun = CalculateNextRun(cachedServiceRunSchedule);
            return nextRun <= dateTimeProvider.CurrentTime;
        }

        public async Task SaveLastRun(string serviceName)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                cachedServiceRunSchedule = await dbContext.ServiceRunSchedules.FirstAsync(x => x.ServiceName == serviceName);
                cachedServiceRunSchedule.LastRun = dateTimeProvider.CurrentTime;
                await dbContext.SaveChangesAsync();
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
    }
}
