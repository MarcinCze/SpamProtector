using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ProtectorLib;
using ProtectorLib.Extensions;
using ProtectorLib.Providers;

namespace DeleteMainService
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
                    .AddDbContext<SpamProtectorDBContext>(options => options.UseSqlServer(hostContext.Configuration.GetConnectionString("SpamProtectorDBContext")))
                    .AddSingleton<IDateTimeProvider, DateTimeProvider>()
                    .AddServiceRunHandlers()
                    .AddHostedService<Worker>();
            });
    }
}
