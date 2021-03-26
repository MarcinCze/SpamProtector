using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProtectorLib.Models;
using RabbitMQ.Client;
using System.Text.Json;

namespace ProtectorLib.Logger
{
    public class RabbitMqLogger : ILogger
    {
        private readonly string name;
        private readonly RabbitMqLoggerConfiguration config;

        public RabbitMqLogger(string name, RabbitMqLoggerConfiguration config)
        {
            this.name = name;
            this.config = config;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //if (!IsEnabled(logLevel))
            //    return;

            //if (config.EventId != 0 && config.EventId != eventId.Id)
            //{
            //    return;
            //}

            //LogEntryDTO entryDto = new()
            //{
            //    Type = logLevel.ToString().ToUpper(),
            //    CreationTime = DateTime.Now
            //};

            //if (state is IReadOnlyList<KeyValuePair<string, object>> formattedLogValues)
            //{
            //    entryDto.AdditionalParameters = ExtractAdditionalArguments(formattedLogValues);
            //}

            //entryDto.Action = ExtractAction(entryDto.AdditionalParameters);
            //entryDto.RemoteIp = ExtractRemoteIp(entryDto.AdditionalParameters);
            //entryDto.HttpMethod = ExtractHttpMethod(entryDto.AdditionalParameters);

            //try
            //{
            //    //SendMessage(entryDto);
            //}
            //catch (Exception)
            //{ }
        }

        private void SendMessage(LogEntryDTO logEntryDTO)
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
                     body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(logEntryDTO)));
        }

        private List<string> ExtractAdditionalArguments(IReadOnlyList<KeyValuePair<string, object>> formattedLogValues)
        {
            try
            {
                var additionalParams = new List<string>();

                FieldInfo fieldInfo = formattedLogValues.GetType().GetField("_values", BindingFlags.NonPublic | BindingFlags.Instance);

                if (fieldInfo == null)
                    return new List<string>();

                if (fieldInfo.GetValue(formattedLogValues) is object[] valuesArr)
                {
                    additionalParams.AddRange(valuesArr.Select(obj => obj.ToString()));
                }

                return additionalParams;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        private string ExtractRemoteIp(IEnumerable<string> parameters) => ExtractParameter(parameters, "RemoteIp");

        private string ExtractHttpMethod(IEnumerable<string> parameters) => ExtractParameter(parameters, "HttpMethod");

        private string ExtractAction(IEnumerable<string> parameters) => ExtractParameter(parameters, "Action");

        private string ExtractParameter(IEnumerable<string> parameters, string paramName)
        {
            try
            {
                if (!parameters.Any())
                    return null;

                string paramLine = parameters.FirstOrDefault(x => x.StartsWith(paramName));

                if (paramLine == null)
                    return null;

                string[] parts = paramLine.Split('#');

                if (parts.Length != 2)
                    return null;

                return parts[1];
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
