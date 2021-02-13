using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using ProtectorLib;
using ProtectorLib.Models;
using ProtectorLib.Models.Enums;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace MessageServiceRunHandlerService
{
    public class ServiceRunHandler : IServiceRunHandler
    {
        IServiceScopeFactory serviceScopeFactory;

        public ServiceRunHandler(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task SaveAsync(ServiceRun message)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                if (message.ServiceStatus == ServiceStatus.PROCESSING)
                {
                    await dbContext.ServiceRunHistories.AddAsync(new ServiceRunHistory
                    {
                        ServiceName = message.ServiceName,
                        Branch = message.Branch,
                        StartTime = message.StartTime,
                        Status = message.Status,
                        ServiceVersion = message.ServiceVersion
                    });
                }
                else
                {
                    var serviceRun = await dbContext.ServiceRunHistories
                        .OrderByDescending(x => x.StartTime)
                        .FirstOrDefaultAsync(x => x.ServiceName == message.ServiceName && x.Branch == message.Branch && x.Status == ServiceStatus.PROCESSING.ToString());

                    if (serviceRun == null)
                    {
                        throw new Exception("ServiceRun entry not found");
                    }

                    serviceRun.EndTime = message.EndTime;
                    serviceRun.Status = message.Status;
                    serviceRun.Information = message.Information;
                    serviceRun.ExecutionTime = message.ExecutionTime;
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
