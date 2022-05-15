using Microsoft.Extensions.DependencyInjection;

namespace SsmsLite.MsSqlDb
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDbServices(this IServiceCollection services)
        {
            services.AddSingleton<SqlDbInfo>();

            return services;
        }
    }
}
