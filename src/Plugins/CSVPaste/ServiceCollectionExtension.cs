using Microsoft.Extensions.DependencyInjection;

namespace SsmsLite.CsvPaste
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSsmsPlusCsvServices(this IServiceCollection services)
        {
            services.AddSingleton<CsvPaste>();

            return services;
        }

    }
}