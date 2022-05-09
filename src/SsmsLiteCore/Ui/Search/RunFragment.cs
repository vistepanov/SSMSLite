using System.Windows;
using System.Windows.Documents;

namespace SsmsLite.Core.Ui.Search
{
    public class RunFragment : Run
    {
        public static readonly DependencyProperty TextFragmentTypeProperty = DependencyProperty.Register(
            "TextFragmentType"
            , typeof(TextFragmentType)
            , typeof(RunFragment)
            , new PropertyMetadata(TextFragmentType.Primary)
        );

        public TextFragmentType TextFragmentType
        {
            get => (TextFragmentType)GetValue(TextFragmentTypeProperty);
            private set => SetValue(TextFragmentTypeProperty, value);
        }

        private RunFragment(string text, TextFragmentType fragmentType) : base(text)
        {
            TextFragmentType = fragmentType;
        }

        public static RunFragment Create(TextFragment fragment)
        {
            return new RunFragment(fragment.Value, fragment.FragmentType);
        }
    }
}