using Microsoft.Extensions.DependencyInjection;
using ProtectorLib.Handlers;
using ProtectorLib.Providers;

namespace ProtectorLib.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddMainMailboxProvider(this IServiceCollection services)
        {
            services
                .AddMailboxRequiredClasses()
                .AddSingleton<IMailboxProvider, MainMailboxProvider>();
            return services;
        }

        public static IServiceCollection AddSpamMailboxProvider(this IServiceCollection services)
        {
            services
                .AddMailboxRequiredClasses()
                .AddSingleton<IMailboxProvider, SpamMailboxProvider>();
            return services;
        }

        public static IServiceCollection AddServiceRunHandlers(this IServiceCollection services)
        {
            services
                .AddSingleton<IServiceRunHistoryHandler, ServiceRunHistoryHandler>()
                .AddSingleton<IServiceRunScheduleProvider, ServiceRunScheduleProvider>();
            return services;
        }

        private static IServiceCollection AddMailboxRequiredClasses(this IServiceCollection services)
        {
            services
                .AddSingleton<IDateTimeProvider, DateTimeProvider>()
                .AddSingleton<IMessagesHandler, MessagesHandler>()
                .AddSingleton<IRulesProvider, RulesProvider>();
            return services;
        }
    }
}
