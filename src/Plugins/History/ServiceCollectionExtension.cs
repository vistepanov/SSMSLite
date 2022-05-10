using Microsoft.Extensions.DependencyInjection;
using SsmsLite.History.Repositories;
using SsmsLite.History.Services;
using SsmsLite.History.UI;

namespace SsmsLite.History
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddHistoryServices(this IServiceCollection services)
        {
            services.AddSingleton<QueryTracker>();
            services.AddSingleton<QueryItemRepository>();
            services.AddSingleton<HistoryPlugin>();

            services.AddSingleton<HistoryUi>();
            services.AddSingleton<HistoryControlVm>();

            return services;
        }
    }
}