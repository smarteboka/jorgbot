using Fpl.Client.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Smartbot.Utilities;
using Smartbot.Utilities.Handlers._4sq.FourSquareServices;
using Smartbot.Utilities.SlackAPIExtensions;
using Smartbot.Utilities.Storage;
using Smartbot.Utilities.Storage.Events;
using Smartbot.Utilities.Storage.SlackUrls;
using Smartbot.Utilities.Storsdager.RecurringActions;

namespace Smartbot
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSmartbot(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddFplApiClient(configuration.GetSection("fpl"));
            services.AddSingleton<SlackChannels>();
            services.AddSingleton<Smartinger>();
            services.Configure<SmartStorageOptions>(configuration);
            services.AddSingleton<SlackMessagesStorage>();

            services.AddSingleton<FourSquareService>();
            services.Configure<FourSquareOptions>(configuration);

            services.AddSingleton<EventsStorage>();
            services.AddSingleton<InvitationsStorage>();

            services.AddSingleton<StorsdagInviter>();
            services.AddSingleton<SlackQuestionClient>();
            return services;
        }
    }
}