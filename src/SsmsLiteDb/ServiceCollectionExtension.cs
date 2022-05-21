using Microsoft.Extensions.DependencyInjection;
using SsmsLite.Core.Database;
using SsmsLite.Db.DbUpdate;

namespace SsmsLite.Db
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDbServices(this IServiceCollection services)
        {
            services.AddSingleton<ILocalDatabase, App.Db>();
            services.AddSingleton<DbUpdater>();

            return services;
        }
    }
}
