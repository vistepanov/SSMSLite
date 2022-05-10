using Demo.Services;
using Microsoft.Extensions.DependencyInjection;
using SsmsLite.Core;
using SsmsLite.Core.App;
using SsmsLite.Core.Integration;
using SsmsLite.Core.Integration.ObjectExplorer;
using SsmsLite.Core.Settings;
using SsmsLite.Db;
using SsmsLite.Document;
using SsmsLite.History;
using SsmsLite.Preferences;
using SsmsLite.Search;
using SsmsLite.Search.Services;

namespace Demo
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInternalServices(this IServiceCollection services)
        {
            services.AddDbServices();
            services.AddCoreServices();
            services.AddHistoryServices();
            services.AddSearchServices();
            services.AddDocumentServices();
            services.AddPreferencesServices();

            services.AddSingleton<IVersionProvider, DemoVersionProvider>();
            services.AddSingleton<IWorkingDirProvider, DemoWorkingDirProvider>();
            services.AddSingleton<IObjectExplorerInteraction, DemoObjectExplorerInteraction>();
            services.AddSingleton<IServiceCacheIntegration, DemoServiceCacheIntegration>();

            // Comment out this line to use the real indexer
            services.AddSingleton<IDbIndexer, DemoDbIndexer>();

            services.AddSingleton(_ => new DistributionSettings { ContributeText = "Contribute", ContributeUrl = "Http://Demo" });
            return services;
        }
    }
}