using Microsoft.Extensions.DependencyInjection;
using SsmsLite.Preferences.UI;

namespace SsmsLite.Preferences
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddPreferencesServices(this IServiceCollection services)
        {
            services.AddSingleton<PreferencesUI>();
            services.AddSingleton<PreferencesWindowVM>();
            return services;
        }
    }
}
