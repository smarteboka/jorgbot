using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Smartbot.Data.Storage;
using Smartbot.Data.Storage.Events;
using Smartbot.Data.Storage.SlackUrls;

namespace Smartbot.Data
{
    public static class ServiceBuilderExtensions
    {
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmartStorageOptions>(configuration);
            
            services.AddSingleton<SlackMessagesStorage>();
            services.AddSingleton<IInvitationsStorage, InvitationsStorage>();
            services.AddSingleton<IEventsStorage,EventsStorage>();
            services.AddSingleton<IInvitationsStorage,InvitationsStorage>();
            
            return services;
        }
    }
}