using Microsoft.Extensions.Logging;

namespace ProtectorLib.Logger
{
    public class RabbitMqLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
        public int EventId { get; set; } = 0;

        public string Host { get; set; }
        public string AccountLogin { get; set; }
        public string AccountPassword { get; set; }
        public string LogQueueName { get; set; }
    }
}
