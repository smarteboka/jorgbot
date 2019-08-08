using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Slackbot.Net.Hosting
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection ConfigureAndValidate<T>(this IServiceCollection @this,
            IConfiguration config) where T : class
            => @this
                .Configure<T>(config)
                .PostConfigure<T>(settings =>
                {
                    var configErrors = settings.ValidationErrors().ToArray();
                    if (configErrors.Any())
                    {
                        var aggrErrors = string.Join(",", configErrors);
                        var count = configErrors.Length;
                        var configType = typeof(T).Name;
                        throw new ConfigurationException(
                            $"Found {count} configuration error(s) in {configType}: {aggrErrors}");
                    }
                });

        internal static IServiceCollection ConfigureAndValidate<T>(this IServiceCollection @this,
            Action<T> config) where T : class
            => @this
                .Configure<T>(config)
                .PostConfigure<T>(settings =>
                {
                    var configErrors = settings.ValidationErrors().ToArray();
                    if (configErrors.Any())
                    {
                        var aggrErrors = string.Join(", ", configErrors);
                        var count = configErrors.Length;
                        var configType = typeof(T).Name;
                        throw new ConfigurationException(
                            $"Found {count} configuration error(s) in {configType}: {aggrErrors}");
                    }
                });
    }

    internal class ConfigurationException : Exception
    {
        public ConfigurationException(string s) : base(s)
        {
        }
    }
}