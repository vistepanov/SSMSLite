using Microsoft.Extensions.DependencyInjection;
using SSMSPlusHistory.Repositories;
using SSMSPlusHistory.Services;
using SSMSPlusHistory.UI;

namespace SSMSPlusHistory
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSsmsPlusHistoryServices(this IServiceCollection services)
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