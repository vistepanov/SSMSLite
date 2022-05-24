using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using SsmsLite.Core.Ui.Search;

namespace SsmsLite.Core.Ui.Extensions
{
    public class FlowDocumentScrollViewerDocumentHighlightBehavior
    {
        public static TextFragments GetHighlightContext(DependencyObject obj)
        {
            return (TextFragments)obj.GetValue(HighlightContextProperty);
        }

        public static void SetHighlightContext(DependencyObject obj, TextFragments value)
        {
            obj.SetValue(HighlightContextProperty, value);
        }

        public static readonly DependencyProperty HighlightContextProperty =
            DependencyProperty.RegisterAttached("HighlightContext",
                typeof(TextFragments),
                typeof(FlowDocumentScrollViewerDocumentHighlightBehavior),
                new PropertyMetadata(null, OnHighlightContextChanged));

        private static void OnHighlightContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FlowDocumentScrollViewer docScrollViewer))
                return;

            if (!(e.NewValue is TextFragments context))
            {
                docScrollViewer.Document = null;
                return;
            }


            var para = new Paragraph();
            RunFragment firstHighlight = default;
            foreach (var fragment in context.Fragments)
            {
                var run = RunFragment.Create(fragment);
                if (firstHighlight == null && run.TextFragmentType == TextFragmentType.Highlight)
                {
                    firstHighlight = run;
                }

                para.Inlines.Add(run);
            }

            docScrollViewer.Document = new FlowDocument(para);
            HandleScrollPosition(docScrollViewer, firstHighlight);
        }

        private static void HandleScrollPosition(FlowDocumentScrollViewer docScrollViewer, Run firstMatch)
        {
            var scrollViewer = FindScrollViewer(docScrollViewer);
            if (scrollViewer == null)
                return;

            scrollViewer.ScrollToTop();

            if (firstMatch != null)
            {
                firstMatch.Dispatcher.InvokeAsync(async () =>
                    {
                        await Task.Delay(10);
                        firstMatch.BringIntoView();
                    }
                );
            }
        }

        private static ScrollViewer FindScrollViewer(FlowDocumentScrollViewer flowDocumentScrollViewer)
        {
            if (VisualTreeHelper.GetChildrenCount(flowDocumentScrollViewer) == 0)
            {
                return null;
            }

            // Border is the first child of first child of a ScrollDocumentViewer
            var firstChild = VisualTreeHelper.GetChild(flowDocumentScrollViewer, 0);

            var border = VisualTreeHelper.GetChild(firstChild, 0) as Decorator;
            if (border == null)
                return null;

            return border.Child as ScrollViewer;
        }
    }
}