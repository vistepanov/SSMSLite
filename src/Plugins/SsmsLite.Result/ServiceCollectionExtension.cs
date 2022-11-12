using Microsoft.Extensions.DependencyInjection;

namespace SsmsLite.Result
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddHistoryServices(this IServiceCollection services)
        {
            services.AddSingleton<ResultSelectionTracker>();
            //services.AddSingleton<QueryItemRepository>();
            //services.AddSingleton<HistoryPlugin>();

            //services.AddSingleton<HistoryUi>();
            //services.AddSingleton<HistoryControlVm>();

            return services;
        }
    }
}