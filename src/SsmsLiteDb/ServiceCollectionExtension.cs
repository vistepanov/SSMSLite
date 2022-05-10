using Microsoft.Extensions.DependencyInjection;
using SsmsLite.Db.DbUpdate;

namespace SsmsLite.Db
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDbServices(this IServiceCollection services)
        {
            services.AddSingleton<DbUpdater>();

            return services;
        }
    }
}
