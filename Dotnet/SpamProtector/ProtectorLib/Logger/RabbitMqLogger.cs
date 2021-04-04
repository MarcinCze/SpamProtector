using Microsoft.Extensions.Logging;

using ProtectorLib.Models;

using RabbitMQ.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace ProtectorLib.Logger
{
    public class RabbitMqLogger : ILogger
    {
        private readonly string name;
        private readonly string version;
        private readonly RabbitMqLoggerConfiguration config;

        public RabbitMqLogger(string name, RabbitMqLoggerConfiguration config, string version)
        {
            this.name = name;
            this.version = version;
            this.config = config;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (config.EventId != 0 && config.EventId != eventId.Id)
                return;

            LogEntryDTO entryDto = new()
            {
                Type = logLevel.ToString(),
                ServiceName = name,
                ServiceVersion = version,
                CreationTime = DateTime.Now,
                Message = formatter(state, exception),
                StackTrace = exception?.StackTrace
            };

            if (state is IReadOnlyList<KeyValuePair<string, object>> formattedLogValues)
            {
                var items = ExtractAdditionalArguments(formattedLogValues);

                if (items.Any()) 
                    entryDto.AdditionalData = JsonSerializer.Serialize(items);
            }

            try
            {
                SendMessage(entryDto);
            }
            catch (Exception)
            { }
        }

        private void SendMessage(LogEntryDTO logEntryDto)
        {
            var factory = new ConnectionFactory()
            {
                HostName = config.Host,
                UserName = config.AccountLogin,
                Password = config.AccountPassword
            };

            using var connection = factory.CreateConnection(nameof(RabbitMqLogger));
            using var channel = connection.CreateModel();

            IBasicProperties props = channel.CreateBasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;

            channel.BasicPublish(exchange: "",
                     routingKey: config.LogQueueName,
                     basicProperties: props,
                     body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(logEntryDto)));
        }

        private List<string> ExtractAdditionalArguments(IReadOnlyList<KeyValuePair<string, object>> formattedLogValues)
        {
            try
            {
                List<string> additionalParams = new();

                FieldInfo fieldInfo = formattedLogValues.GetType().GetField("_values", BindingFlags.NonPublic | BindingFlags.Instance);

                if (fieldInfo == null)
                    return null;

                if (fieldInfo.GetValue(formattedLogValues) is object[] valuesArr)
                    additionalParams.AddRange(valuesArr.Select(obj => obj.ToString()));

                return additionalParams;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
