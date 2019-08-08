using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Smartbot.Utilities;
using Smartbot.Utilities.FourSquareServices;
using Smartbot.Utilities.Storage;

namespace Smartbot
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSmartbot(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<Smartinger>();
            services.Configure<SmartStorageOptions>(configuration);
            services.AddSingleton<SlackMessagesStorage>();

            services.AddSingleton<FourSquareService>();
            services.Configure<FourSquareOptions>(configuration);
            return services;
        }
    }
}