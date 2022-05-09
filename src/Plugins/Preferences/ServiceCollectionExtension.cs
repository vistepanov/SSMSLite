using Microsoft.Extensions.DependencyInjection;
using SSMSPlusPreferences.UI;

namespace SSMSPlusPreferences
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSSMSPlusPreferencesServices(this IServiceCollection services)
        {
            services.AddSingleton<PreferencesUI>();
            services.AddSingleton<PreferencesWindowVM>();
            return services;
        }
    }
}
