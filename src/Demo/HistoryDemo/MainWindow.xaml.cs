using Demo.Settings;
using System.Windows;
using SsmsLite.Core.Di;
using SsmsLite.Core.Integration.Connection;
using SsmsLite.Core.Ui.Extensions;
using SsmsLite.Document.UI;
using SsmsLite.History.UI;
using SsmsLite.Preferences.UI;
using SsmsLite.Search.UI;

namespace Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DemoDbSettings settings = ServiceLocator.GetRequiredService<DemoDbSettings>();
            var dbConnectionStr = new DbConnectionString(settings.ConnectionString, settings.DbName);

            var tested = 0;
            if (tested == 0)
            {
                var histo = new HistoryControl();
                Panel.Children.Add(histo);
            }
            else if (tested == 1)
            {
                var search = new SchemaSearchControl();
                search.Initialize(dbConnectionStr);
                Panel.Children.Add(search);
            }
            else if (tested == 2)
            {
                var docs = new ExportDocumentsControl();
                docs.Initialize(dbConnectionStr);
                Panel.Children.Add(docs);
            }
            else if (tested == 3)
            {
                var ui = new PreferencesWindow();
                ui.ShowAndActivate();
            }
        }
    }
}
