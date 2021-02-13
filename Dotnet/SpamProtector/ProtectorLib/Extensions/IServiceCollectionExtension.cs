using Microsoft.Extensions.DependencyInjection;
using ProtectorLib.Controllers;
using ProtectorLib.Handlers;
using ProtectorLib.Messaging;
using ProtectorLib.Providers;

namespace ProtectorLib.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddMailboxProviders(this IServiceCollection services)
        {
            services
                .AddSingleton<IMailboxProvider, MainMailboxProvider>()
                .AddSingleton<IMailboxProvider, AdsMailboxProvider>()
                .AddSingleton<IMailboxProvider, WorkMailboxProvider>()
                .AddSingleton<IMailboxProvider, SpamMailboxProvider>();
            
            return services;
        }

        public static IServiceCollection AddMailboxRequiredClasses(this IServiceCollection services)
        {
            services
                .AddSingleton<IDateTimeProvider, DateTimeProvider>()
                .AddSingleton<IMessagesHandler, MessagesHandler>()
                .AddSingleton<IRulesProvider, RulesProvider>();
            return services;
        }

        public static IServiceCollection AddServiceRunHandlers(this IServiceCollection services)
        {
            services
                //.AddSingleton<IServiceRunHistoryHandler, ServiceRunHistoryHandler>()
                .AddSingleton<IServiceRunHistoryHandler, ServiceRunHistoryMsgSender>()
                .AddSingleton<IServiceRunScheduleProvider, ServiceRunScheduleProvider>();
            return services;
        }

        public static IServiceCollection AddMailboxController(this IServiceCollection services)
        {
            services.AddSingleton<IMailboxController, MailboxController>();
            return services;
        }

        public static IServiceCollection AddMessagingMechanism(this IServiceCollection services)
        {
            services.AddSingleton<IMessagingService, MessagingService>();
            return services;
        }
    }
}
