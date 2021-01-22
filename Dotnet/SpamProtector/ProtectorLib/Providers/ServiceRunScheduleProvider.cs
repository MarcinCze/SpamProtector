using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;

namespace ProtectorLib.Providers
{
    public class ServiceRunScheduleProvider : IServiceRunScheduleProvider
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public ServiceRunScheduleProvider(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public bool ShouldRun(string serviceName)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();
                var nextRun = CalculateNextRun(dbContext.ServiceRunSchedules.First(x => x.ServiceName == serviceName));

                return nextRun <= DateTime.Now;
            }
        }

        protected DateTime CalculateNextRun(ServiceRunSchedule service)
        {
            if (!service.LastRun.HasValue)
                return DateTime.Now;

            return service.LastRun.Value
                .AddDays(service.RunEveryDays)
                .AddHours(service.RunEveryHours)
                .AddMinutes(service.RunEveryMinutes)
                .AddSeconds(service.RunEverySeconds);
        }
    }
}
