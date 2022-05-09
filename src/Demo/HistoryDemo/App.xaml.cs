using Demo.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using SsmsLite.Core.Di;
using SsmsLite.Db.DbUpdate;

namespace Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IConfigurationRoot _configuration;
        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                _configuration = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddXmlFile(@"_settings.config", optional: false)
                               .Build();

                // create service collection
                var services = new ServiceCollection();
                ConfigureServices(services);

                _serviceProvider = services.BuildServiceProvider();
                ServiceLocator.SetLocatorProvider(_serviceProvider);

                _serviceProvider.GetRequiredService<DbUpdater>().UpdateDb();
            }
            catch (Exception ex)
            {
                _serviceProvider.GetRequiredService<ILogger<App>>().LogCritical(ex, "Critical Error When starting plugin");
                throw;
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "log");


            services.ConfigureSection<DemoDbSettings>(_configuration.GetSection("DemoDb"));

            // add services:
            services.AddInternalServices();
        }
    }
}
