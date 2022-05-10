using Microsoft.Extensions.DependencyInjection;
using SsmsLite.Document.UI;

namespace SsmsLite.Document
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDocumentServices(this IServiceCollection services)
        {
            services.AddSingleton<DocumentUi>();
            services.AddSingleton<DocumentPlugin>();
            services.AddTransient<ExportDocumentsControlVm>();
            return services;
        }
    }
}