using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ProtectorLib;
using ProtectorLib.Configuration;
using ProtectorLib.Extensions;

namespace DeleteSpamService
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
            .ConfigureServices((hostContext, services) =>
            {
                services
                    .AddSingleton(hostContext.Configuration.GetSection("Mailboxes").GetSection("SpamBox").Get<MailboxConfig>())
                    .AddSingleton(hostContext.Configuration.GetSection("Services").Get<ServicesConfig>())
                    .AddDbContext<SpamProtectorDBContext>(options => options.UseSqlServer(hostContext.Configuration.GetConnectionString("SpamProtectorDBContext")))
                    .AddSpamMailboxProvider()
                    .AddServiceRunHandlers()
                    .AddHostedService<Worker>();
            });
    }
}
