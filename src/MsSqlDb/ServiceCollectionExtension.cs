using Microsoft.Extensions.DependencyInjection;

namespace SsmsLite.MsSqlDb
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSqlDbServices(this IServiceCollection services)
        {
            services.AddSingleton<SqlDbInfo>();

            return services;
        }
    }
}
