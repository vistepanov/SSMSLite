using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using SsmsLite.Core.Di;
using SsmsLite.Core.Integration.Connection;
using SsmsLite.Core.Ui;

namespace SSMSPlusSearch.UI
{
    /// <summary>
    /// Interaction logic for Simple.xaml
    /// </summary>
    public partial class SchemaSearchControl : UserControl, IDisposable
    {
        public SchemaSearchControl()
        {
            InitializeComponent();
            Loaded += SchemaSearchControl_Loaded;
        }

        private void SchemaSearchControl_Loaded(object sender, RoutedEventArgs e)
        {
            Filter.Focus();
            Loaded -= SchemaSearchControl_Loaded;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new EmptyAutomationPeer(this);
        }

        private SchemaSearchControlVm ViewModel => this.DataContext as SchemaSearchControlVm;

        public void Initialize(DbConnectionString cnxStr)
        {
            var viewModel = ServiceLocator.GetRequiredService<SchemaSearchControlVm>();
            this.DataContext = viewModel;
            this.Dispatcher.Invoke(() => viewModel.InitializeDbAsync(cnxStr));
        }

        public void Dispose()
        {
            BindingOperations.ClearAllBindings(this);
            ViewModel?.Free();
            this.DataContext = null;
        }
    }
}
