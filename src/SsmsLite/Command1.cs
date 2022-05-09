using System;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Windows;
using EnvDTE80;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Shell;
using SsmsLite.Core;
using SsmsLite.Core.App;
using SsmsLite.Core.Di;
using SsmsLite.Core.Integration;
using SsmsLite.Core.Settings;
using SsmsLite.Core.Utils;
using SsmsLite.Core.Utils.Logging;
using SsmsLite.CsvPaste;
using SsmsLite.Db;
using SsmsLite.Db.DbUpdate;
using SSMSPlus.Services;
using SSMSPlusDocument;
using SSMSPlusHistory;
using SSMSPlusPreferences;
using SSMSPlusSearch;
using ServiceProvider = Microsoft.Extensions.DependencyInjection.ServiceProvider;
using Task = System.Threading.Tasks.Task;


namespace SSMSPlus
{
    /// <summary>
    ///     Command handler
    /// </summary>
    internal sealed class Command1
    {
        private static DTE2 _dte;
        private static AsyncPackage _asyncPackage;
        private static OleMenuCommandService _commandService;


        private readonly ServiceProvider _serviceProvider;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Command1" /> class.
        ///     Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        private Command1()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var builder = new ConfigurationBuilder()
                        .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                        .AddXmlFile(@"_settings.config", true)
                    //.AddJsonFile()
//                    .Build()
                    ;
                var services = new ServiceCollection();

                ConfigureLogging(services);
                services.ConfigureSection<DistributionSettings>(builder.Build().GetSection("Distribution"));
                services.AddSingleton(_ => new PackageProvider(_dte, _asyncPackage, _commandService));

                // add services:
                AddInternalServices(services);

                _serviceProvider = services.BuildServiceProvider();
                ServiceLocator.SetLocatorProvider(_serviceProvider);

                _serviceProvider.GetRequiredService<DbUpdater>().UpdateDb();
                _serviceProvider.GetRequiredService<HistoryPlugin>().Register();
                _serviceProvider.GetRequiredService<SearchPlugin>().Register();
                _serviceProvider.GetRequiredService<DocumentPlugin>().Register();
                _serviceProvider.GetRequiredService<PreferencesUI>().Register();
                _serviceProvider.GetRequiredService<CsvPaste>().Register();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetFullStackTraceWithMessage(), "Could not Load SSMSPlus");
                _serviceProvider?.GetRequiredService<ILogger<Command1>>()
                    .LogCritical(ex, "Critical Error when starting plugin");

                throw;
            }
        }

        private void ConfigureLogging(ServiceCollection services)
        {
            try
            {
                var logPath = Path.Combine(new SsmsWorkingDirProvider().GetWorkingDir(), "log");


                // configure logging
                services.AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddFileLogger(() => new FileLoggerOptions
                    {
                        Folder = logPath,
                        LogLevel = LogLevel.Warning,
                        RetainPolicyFileCount = 30
                    });
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetFullStackTraceWithMessage(), "Logger failed to load");

                _serviceProvider?.GetRequiredService<ILogger<Command1>>()
                    .LogCritical(ex, "Critical Error when starting plugin");

                throw;
            }
        }

        /// <summary>
        ///     Gets the instance of the command.
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public static Command1 Instance { get; private set; }


        private static void AddInternalServices(IServiceCollection services)
        {
            services.AddSingleton<IWorkingDirProvider, SsmsWorkingDirProvider>();
            services.AddSingleton<IVersionProvider, VersionProvider>();

            services.AddSsmsPlusCsvServices();
            services.AddSSMSPlusDbServices(); // Db initialize & update
            services.AddSSMSPlusCoreServices(); // CORE services
            services.AddSsmsPlusHistoryServices(); // History
            services.AddSsmsPlusSearchServices(); // Search
            services.AddSsmsPlusDocumentServices(); // Document
            services.AddSSMSPlusPreferencesServices(); //

        }

        /// <summary>
        ///     Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="dte"></param>
        public static async Task InitializeAsync(AsyncPackage package, DTE2 dte)
        {
            // Switch to the main thread - the call to AddCommand in Command1's constructor requires the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            _dte = dte;
            _asyncPackage = package;
            _commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

            Instance = new Command1();
        }
    }
}