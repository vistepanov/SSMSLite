using System.Windows;
using System.Windows.Controls;
using SsmsLite.Core.Di;

namespace SSMSPlusHistory.UI
{
    /// <summary>
    /// Interaction logic for Simple.xaml
    /// </summary>
    public partial class HistoryControl : UserControl
    {
        public HistoryControl()
        {
            InitializeComponent();
            this.DataContext = ServiceLocator.GetRequiredService<HistoryControlVm>();
            this.Loaded += HistoryControl_Loaded;
        }

        private void HistoryControl_Loaded(object sender, RoutedEventArgs e)
        {
            QueryFilter.Focus();
            this.Loaded -= HistoryControl_Loaded;
        }
    }
}
