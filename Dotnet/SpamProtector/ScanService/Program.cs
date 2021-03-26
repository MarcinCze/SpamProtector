using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProtectorLib;
using ProtectorLib.Configuration;
using ProtectorLib.Extensions;
using ProtectorLib.Logger;
using System.IO;

namespace ScanService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var sharedFolder = Path.Combine(hostContext.HostingEnvironment.ContentRootPath, "..", "Shared");
                    config.AddJsonFile(Path.Combine(sharedFolder, "appsettings.json"), optional: true);
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddProvider(
                        new RabbitMqLoggerProvider(
                            hostContext.Configuration.GetSection("Messaging").Get<RabbitMqLoggerConfiguration>(),
                            typeof(Worker).Assembly.GetName().Version?.ToString()
                        ));
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton(hostContext.Configuration.GetSection("Mailboxes").Get<MailboxesConfig>())
                        .AddSingleton(hostContext.Configuration.GetSection("Services").Get<ServicesConfig>())
                        .AddSingleton(hostContext.Configuration.GetSection("Messaging").Get<MessagingConfig>())
                        .AddDbContext<SpamProtectorDBContext>(options => options.UseSqlServer(hostContext.Configuration.GetConnectionString("SpamProtectorDBContext")))
                        .AddMailboxController()
                        .AddMailboxProviders()
                        .AddMailboxRequiredClasses()
                        .AddServiceRunHandlers()
                        .AddMessagingMechanism()
                        .AddHostedService<Worker>();
                });
    }
}
