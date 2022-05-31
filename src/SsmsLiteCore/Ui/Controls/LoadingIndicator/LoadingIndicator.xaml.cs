using System.Windows;

namespace SsmsLite.Core.Ui.Controls.LoadingIndicator
{
    /// <summary>
    /// Interaction logic for LoadingIndicator.xaml
    /// </summary>
    public partial class LoadingIndicator
    {
        public static readonly DependencyProperty TitleProperty =
     DependencyProperty.Register(
    "Title",
    typeof(string),
    typeof(LoadingIndicator),
    new PropertyMetadata(null));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public LoadingIndicator()
        {
            InitializeComponent();
        }
    }
}
