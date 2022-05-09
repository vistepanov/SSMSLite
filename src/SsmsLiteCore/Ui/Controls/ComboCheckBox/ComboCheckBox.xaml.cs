using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SsmsLite.Core.Ui.Controls.ComboCheckBox
{
    /// <summary>
    /// Interaction logic for ComboCheckBox.xaml
    /// </summary>
    public partial class ComboCheckBox : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
         DependencyProperty.Register(
        "ViewModel",
        typeof(IComboCheckBoxViewModel),
        typeof(ComboCheckBox),
        new PropertyMetadata(null));

        public IComboCheckBoxViewModel ViewModel
        {
            get => (IComboCheckBoxViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public ComboCheckBox()
        {
            InitializeComponent();
        }

        private void StackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is CheckBox))
                e.Handled = true;
        }
    }
}
