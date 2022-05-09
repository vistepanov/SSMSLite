using Microsoft.Extensions.DependencyInjection;
using SSMSPlusSearch.Repositories;
using SSMSPlusSearch.Services;
using SSMSPlusSearch.UI;

namespace SSMSPlusSearch
{
    public static class ServiceCollectionExtension
    {

        public static IServiceCollection AddSsmsPlusSearchServices(this IServiceCollection services)
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