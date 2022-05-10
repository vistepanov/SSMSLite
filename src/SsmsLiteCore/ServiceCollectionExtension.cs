using Microsoft.Extensions.DependencyInjection;
using SsmsLite.Core.Database;
using SsmsLite.Core.Integration;
using SsmsLite.Core.Integration.Connection;
using SsmsLite.Core.Integration.ObjectExplorer;

namespace SsmsLite.Core
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            //services.AddSingleton<DbConnectionFactory>();
            services.AddSingleton<Db>();
            services.AddSingleton<IObjectExplorerInteraction, ObjectExplorerInteraction>();
            services.AddSingleton<IServiceCacheIntegration, ServiceCacheIntegration>();
            services.AddSingleton<DbConnectionProvider>();

            return services;
        }
    }
}
