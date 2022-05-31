using System.Windows.Documents;
using System.Windows.Media;

namespace SsmsLite.Core.Ui.Extensions
{
    public static class RunExtensions
    {
        public static Run CreateColored(string text, Brush foreground)
        {
            var run = new Run(text)
            {
                Foreground = foreground
            };
            return run;
        }

        public static Run CreateRed(string text)
        {
            return CreateColored(text, Brushes.Red);
        }

        public static Run CreateWhite(string text)
        {
            return CreateColored(text, Brushes.White);
        }

        public static Run CreateGreen(string text)
        {
            return CreateColored(text, Brushes.Green);
        }
    }
}
