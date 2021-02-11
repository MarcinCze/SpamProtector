using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace ProtectorLib.Handlers
{
    public class ServiceRunHistoryMsgSender : IServiceRunHistoryHandler
    {
        public Task RegisterFinishAsync(string serviceName, string additionalData, ServiceRunHistoryHandler.ServiceStatus endStatus, string executionTime)
        {
            throw new NotImplementedException();
        }

        public Task RegisterFinishAsync(string serviceName, string branchName, string additionalData, ServiceRunHistoryHandler.ServiceStatus endStatus, string executionTime)
        {
            throw new NotImplementedException();
        }

        public async Task RegisterStartAsync(string serviceName, string serviceVersion)
        {
            await RegisterStartAsync(serviceName, serviceVersion, null);
        }

        public Task RegisterStartAsync(string serviceName, string serviceVersion, string branchName)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "SRV-RATEL" };
                factory.UserName = "ratel";
                factory.Password = "janosik";
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    string message = "Hello World!";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "hello",
                                         basicProperties: null,
                                         body: body);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return Task.CompletedTask;
        }
    }
}
