using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ProtectorLib.Models.Enums;
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
        protected abstract TimeSpan StartDelay { get; }

        protected abstract Task ExecuteBodyAsync();

        protected virtual async Task<bool> ShouldItRunAsync() =>
            await serviceRunScheduleProvider.ShouldRunAsync(ServiceName);

        protected virtual async Task SaveStartAsync()
            => await serviceRunHistoryHandler.RegisterStartAsync(ServiceName, ServiceVersion);

        protected virtual async Task SaveFinishAsync(ServiceStatus status, string executionTime)
            => await serviceRunHistoryHandler.RegisterFinishAsync(ServiceName, ServiceResultAdditionalInfo, status, executionTime);

        protected virtual async Task SaveLastRunAsync()
            => await serviceRunScheduleProvider.SaveLastRunAsync(ServiceName);

        protected virtual void FinishActions()
        { }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stopWatch = new Stopwatch();
            await Task.Delay(StartDelay, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (await ShouldItRunAsync())
                {
                    logger.LogInformation("Service should run. Starting operation");
                    stopWatch.Start();

                    var status = ServiceStatus.PROCESSING;
                    await SaveStartAsync();
                    logger.LogInformation("Service start saved");

                    try
                    {
                        ServiceResultAdditionalInfo = null;
                        await ExecuteBodyAsync();
                        status = ServiceStatus.DONE;
                        logger.LogInformation("Service function body executed successfully");
                    }
                    catch (Exception ex)
                    {
                        status = ServiceStatus.ERROR;
                        ServiceResultAdditionalInfo = JsonSerializer.Serialize(new { ex.Message, ex.StackTrace });
                        logger.LogError(ex, $"{ServiceName} throwed an error of type {ex.GetType()}");
                    }
                    finally
                    {
                        stopWatch.Stop();

                        if (status != ServiceStatus.ERROR)
                            await SaveLastRunAsync();
                        
                        logger.LogInformation("Service LastRun saved");
                        await SaveFinishAsync(status, $"{stopWatch.ElapsedMilliseconds} ms");
                        logger.LogInformation("Service finish saved");
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
