using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ProtectorLib.Configuration;
using ProtectorLib.Models;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MessageServiceRunHandlerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IServiceRunHandler serviceRunHandler;

        private MessagingConfig msgConfig;
        private IConnection connection;
        private IModel channel;

        public Worker(ILogger<Worker> logger, MessagingConfig msgConfig, IServiceRunHandler serviceRunHandler)
        {
            this.logger = logger;
            this.msgConfig = msgConfig;
            this.serviceRunHandler = serviceRunHandler;
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

            connection = factory.CreateConnection(nameof(MessageServiceRunHandlerService));
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
                var msgObj = JsonSerializer.Deserialize<QueueMessage>(message);
                var content = JsonSerializer.Deserialize<ServiceRunDTO>(msgObj.Content);
                logger.LogInformation($"Handling incoming message. Service: {content.ServiceName} Branch: {content.Branch} Status: {content.Status}");
                await serviceRunHandler.SaveAsync(content);

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }
    }
}
