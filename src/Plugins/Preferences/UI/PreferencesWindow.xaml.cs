using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using SsmsLite.Core.Di;
using SsmsLite.Core.Ui;

namespace SSMSPlusPreferences.UI
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class PreferencesWindow : Window
    {
        public PreferencesWindow()
        {
            InitializeComponent();
            var viewModel = ServiceLocator.GetRequiredService<PreferencesWindowVM>();
            DataContext = viewModel;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new EmptyAutomationPeer(this);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }
    }
}
