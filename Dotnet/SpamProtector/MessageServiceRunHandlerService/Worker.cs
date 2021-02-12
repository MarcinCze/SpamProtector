using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using ProtectorLib.Models;
using ProtectorLib.Configuration;
using ProtectorLib;
using Microsoft.Extensions.DependencyInjection;
using ProtectorLib.Providers;

namespace MessageServiceRunHandlerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IDateTimeProvider dateTimeProvider;

        private MessagingConfig msgConfig;
        private IConnection connection;
        private IModel channel;

        public Worker(ILogger<Worker> logger, MessagingConfig msgConfig, IServiceScopeFactory serviceScopeFactory, IDateTimeProvider dateTimeProvider)
        {
            this.logger = logger;
            this.msgConfig = msgConfig;
            this.dateTimeProvider = dateTimeProvider;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CreateConnection();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            CloseConnection();
            return base.StopAsync(cancellationToken);
        }

        protected void CreateConnection()
        {
            var factory = new ConnectionFactory() { HostName = msgConfig.Host };
            factory.UserName = msgConfig.AccountLogin;
            factory.Password = msgConfig.AccountPassword;

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            channel.BasicConsume(queue: "queue_service_run", autoAck: false, consumer: consumer);
            
        }

        protected void CloseConnection()
        {
            channel.Close();
            channel.Dispose();
            connection.Close();
            connection.Dispose();
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var task = Task.Run(() => HandleMessage(Encoding.UTF8.GetString(e.Body.ToArray())));

            if (task.Result)
                channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
            else
                channel.BasicNack(deliveryTag: e.DeliveryTag, multiple: false, requeue: true);
        }

        protected async Task<bool> HandleMessage(string message)
        {
            try
            {
                var msgObj = JsonSerializer.Deserialize<ProtectorLib.Models.Message>(message);
                var content = JsonSerializer.Deserialize<ServiceRun>(msgObj.Content);
                logger.LogInformation($"ServiceName from MSG: {content.ServiceName} STATUS: {content.Status}");

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();
                    // TODO FIX
                    if (content.Status != "PROCESSING")
                    {
                        var entry = new ServiceRunHistory
                        {
                            ServiceName = content.ServiceName,
                            ServiceVersion = content.ServiceVersion,
                            Branch = content.Branch,
                            Status = content.Status,
                            StartTime = DateTime.Now,
                            EndTime = content.EndTime,
                            ExecutionTime = content.ExecutionTime,
                            Information = content.Information
                        };

                        await dbContext.ServiceRunHistories.AddAsync(entry);
                    }
                    
                    await dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
