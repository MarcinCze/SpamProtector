using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProtectorLib.Handlers;
using ProtectorLib.Providers;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ProtectorLib.Services
{
    public abstract class ExtendedBackgroundService : BackgroundService
    {
        protected readonly ILogger logger;
        protected readonly IServiceRunHistoryHandler serviceRunHistoryHandler;
        protected readonly IServiceRunScheduleProvider serviceRunScheduleProvider;

        protected string ServiceResultAdditionalInfo { get; set; }
        
        protected ExtendedBackgroundService(
            ILogger logger,
            IServiceRunHistoryHandler serviceRunHistoryHandler,
            IServiceRunScheduleProvider serviceRunScheduleProvider)
        {
            this.logger = logger;
            this.serviceRunScheduleProvider = serviceRunScheduleProvider;
            this.serviceRunHistoryHandler = serviceRunHistoryHandler;
        }

        protected abstract string ServiceName { get; }
        protected abstract string ServiceVersion { get; }

        protected abstract Task ExecuteBodyAsync();

        protected virtual async Task<bool> ShouldItRunAsync() =>
            await serviceRunScheduleProvider.ShouldRunAsync(ServiceName);

        protected virtual async Task SaveStartAsync()
            => await serviceRunHistoryHandler.RegisterStartAsync(ServiceName, ServiceVersion);

        protected virtual async Task SaveFinishAsync(ServiceRunHistoryHandler.ServiceStatus status, string executionTime)
            => await serviceRunHistoryHandler.RegisterFinishAsync(ServiceName, ServiceResultAdditionalInfo, status, executionTime);

        protected virtual async Task SaveLastRunAsync()
            => await serviceRunScheduleProvider.SaveLastRunAsync(ServiceName);

        protected virtual void FinishActions()
        { }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stopWatch = new Stopwatch();

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                if (await ShouldItRunAsync())
                {
                    logger.LogInformation("Started operation");
                    stopWatch.Start();

                    var status = ServiceRunHistoryHandler.ServiceStatus.PROCESSING;
                    await SaveStartAsync();

                    try
                    {
                        ServiceResultAdditionalInfo = null;
                        await ExecuteBodyAsync();
                        status = ServiceRunHistoryHandler.ServiceStatus.DONE;
                    }
                    catch (Exception ex)
                    {
                        status = ServiceRunHistoryHandler.ServiceStatus.ERROR;
                        ServiceResultAdditionalInfo = JsonSerializer.Serialize(new { ex.Message, ex.StackTrace });
                    }
                    finally
                    {
                        stopWatch.Stop();
                        await SaveLastRunAsync();
                        await SaveFinishAsync(status, $"{stopWatch.ElapsedMilliseconds} ms");
                        stopWatch.Reset();
                    }

                    logger.LogInformation("Service run done");
                }

                FinishActions();

                await Task.Delay(new TimeSpan(0, 0, 30), stoppingToken);
            }
        }
    }
}
