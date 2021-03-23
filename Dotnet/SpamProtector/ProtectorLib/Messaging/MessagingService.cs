using ProtectorLib.Configuration;
using ProtectorLib.Models;

using RabbitMQ.Client;

using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace ProtectorLib.Messaging
{
    public class MessagingService : IMessagingService
    {
        private readonly MessagingConfig config;

        public MessagingService(MessagingConfig config)
        {
            this.config = config;
        }

        public void SendMessage(ServiceRunDTO serviceRunMessage)
        {
            PushMessage(new QueueMessage
            {
                MessageType = MessageType.ServiceRun,
                Content = JsonSerializer.Serialize(serviceRunMessage)
            });
        }

        public void SendMessage(EmailDTO emailMessage)
        {
            PushMessage(new QueueMessage
            {
                MessageType = MessageType.Email,
                Content = JsonSerializer.Serialize(emailMessage)
            });
        }

        public void SendMessage(UsedRuleDTO usedRuleMessage)
        {
            PushMessage(new QueueMessage
            {
                MessageType = MessageType.UsedRule,
                Content = JsonSerializer.Serialize(usedRuleMessage)
            });
        }

        public void SendMessages(IEnumerable<EmailDTO> emailMessages)
        {
            PushMessages(
                emailMessages.Select(email => new QueueMessage
                {
                    Content = JsonSerializer.Serialize(email),
                    MessageType = MessageType.Email
                }).ToList()
            );
        }

        private void PushMessage(QueueMessage message)
        {
            PushMessages(new List<QueueMessage>() { message });
        }

        private void PushMessages(IEnumerable<QueueMessage> messages)
        {
            if (!messages.Any())
                return;

            var factory = new ConnectionFactory()
            {
                HostName = config.Host,
                UserName = config.AccountLogin,
                Password = config.AccountPassword
            };

            using var connection = factory.CreateConnection(nameof(ProtectorLib.Messaging.MessagingService));
            using var channel = connection.CreateModel();

            IBasicProperties props = channel.CreateBasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;

            foreach (var msg in messages)
            {
                channel.BasicPublish(
                    exchange: config.ExchangeName,
                    routingKey: msg.MessageType.ToString(),
                    basicProperties: props,
                    body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg)));
            }
        }
    }
}
