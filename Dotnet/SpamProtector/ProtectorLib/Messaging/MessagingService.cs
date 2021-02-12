using ProtectorLib.Configuration;
using ProtectorLib.Models;

using RabbitMQ.Client;

using System.Text;
using System.Text.Json;

namespace ProtectorLib.Messaging
{
    public class MessagingService : IMessagingService
    {
        private readonly MessagingConfig config;

        public MessagingService(MessagingConfig config)
        {
            this.config = config;
        }

        public void SendMessage(ServiceRun serviceRunMessage)
        {
            PushMessage(
                JsonSerializer.Serialize(new Models.Message()
                {
                    MessageType = MessageType.ServiceRun,
                    Content = JsonSerializer.Serialize(serviceRunMessage)
                }), 
                MessageType.ServiceRun);
        }

        public void SendMessage(Email emailMessage)
        {
            PushMessage(
                JsonSerializer.Serialize(new Models.Message()
                {
                    MessageType = MessageType.Email,
                    Content = JsonSerializer.Serialize(emailMessage)
                }),
                MessageType.Email);
        }

        public void SendMessage(UsedRule usedRuleMessage)
        {
            PushMessage(
                JsonSerializer.Serialize(new Models.Message()
                {
                    MessageType = MessageType.UsedRule,
                    Content = JsonSerializer.Serialize(usedRuleMessage)
                }),
                MessageType.UsedRule);
        }

        private void PushMessage(string messageBody, MessageType msgType)
        {
            var factory = new ConnectionFactory() { HostName = config.Host };
            factory.UserName = config.AccountLogin;
            factory.Password = config.AccountPassword;
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.BasicPublish(exchange: config.ExchangeName,
                    routingKey: msgType.ToString(),
                    basicProperties: null,
                    body: Encoding.UTF8.GetBytes(messageBody));
            }
        }
    }
}
