using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ProtectorLib.Logger
{
    public class RabbitMqLoggerProvider : ILoggerProvider
    {
        private readonly string version;
        private readonly RabbitMqLoggerConfiguration config;
        private readonly ConcurrentDictionary<string, RabbitMqLogger> loggers = new();

        public RabbitMqLoggerProvider(RabbitMqLoggerConfiguration config, string assemblyVersion)
        {
            this.config = config;
            version = $"ver {assemblyVersion} / lib {GetType().Assembly.GetName().Version}";
        }

        public ILogger CreateLogger(string categoryName) =>
            loggers.GetOrAdd(categoryName, name => new RabbitMqLogger(name, config, version));

        public void Dispose()
        {
            loggers.Clear();
        }
    }
}
