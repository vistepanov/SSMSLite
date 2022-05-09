using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using SsmsLite.Core.Ui.Search;

namespace SsmsLite.Core.Ui.Converters
{
    public class HighlightToTextBlockConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var highlight = value as TextFragments;
            var tb = new TextBlock();
            foreach (var fragment in highlight.Fragments)
            {
                var run = RunFragment.Create(fragment);
                tb.Inlines.Add(run);
            }
            return tb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}
