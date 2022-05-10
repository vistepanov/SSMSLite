using Microsoft.Extensions.DependencyInjection;

namespace SsmsLite.Sync
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSyncServices(this IServiceCollection services)
        {
            services.AddSingleton<Sync>();

            return services;
        }

    }
}