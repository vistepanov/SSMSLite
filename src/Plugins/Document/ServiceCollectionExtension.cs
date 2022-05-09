using Microsoft.Extensions.DependencyInjection;
using SSMSPlusDocument.UI;

namespace SSMSPlusDocument
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSsmsPlusDocumentServices(this IServiceCollection services)
        {
            services.AddSingleton<DocumentUi>();
            services.AddSingleton<DocumentPlugin>();
            services.AddTransient<ExportDocumentsControlVm>();
            return services;
        }
    }
}