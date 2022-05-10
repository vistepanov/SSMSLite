using Microsoft.Extensions.DependencyInjection;
using SsmsLite.Search.Repositories;
using SsmsLite.Search.Services;
using SsmsLite.Search.UI;

namespace SsmsLite.Search
{
    public static class ServiceCollectionExtension
    {

        public static IServiceCollection AddSearchServices(this IServiceCollection services)
        {
            services.AddSingleton<SearchPlugin>();
            services.AddSingleton<SearchUi>();
            services.AddSingleton<IDbIndexer, DbIndexer>();
            services.AddSingleton<SchemaSearchRepository>();
            services.AddTransient<SchemaSearchControlVm>();
            return services;
        }
    }
}