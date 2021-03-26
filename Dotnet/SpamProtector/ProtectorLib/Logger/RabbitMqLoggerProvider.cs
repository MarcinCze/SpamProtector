using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ProtectorLib.Logger
{
    public class RabbitMqLoggerProvider : ILoggerProvider
    {
        private readonly RabbitMqLoggerConfiguration config;
        private readonly ConcurrentDictionary<string, RabbitMqLogger> loggers = new();

        public RabbitMqLoggerProvider(RabbitMqLoggerConfiguration config)
        {
            this.config = config;
        }

        public ILogger CreateLogger(string categoryName) =>
            loggers.GetOrAdd(categoryName, name => new RabbitMqLogger(name, config));

        public void Dispose()
        {
            loggers.Clear();
        }
    }
}
