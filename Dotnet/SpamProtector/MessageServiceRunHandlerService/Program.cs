using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ProtectorLib;
using ProtectorLib.Configuration;
using ProtectorLib.Providers;

using System.IO;

namespace MessageServiceRunHandlerService
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
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddEventLog();
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var sharedFolder = Path.Combine(hostContext.HostingEnvironment.ContentRootPath, "..", "Shared");
                    config.AddJsonFile(Path.Combine(sharedFolder, "appsettings.json"), optional: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton(hostContext.Configuration.GetSection("Messaging").Get<MessagingConfig>())
                        .AddDbContext<SpamProtectorDBContext>(options => options.UseSqlServer(hostContext.Configuration.GetConnectionString("SpamProtectorDBContext")))
                        .AddSingleton<IDateTimeProvider, DateTimeProvider>()
                        .AddHostedService<Worker>();
                });
    }
}
