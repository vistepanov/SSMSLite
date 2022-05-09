using System;
using System.Windows.Documents;
using System.Windows.Media;
using SsmsLite.Core.Ui.Extensions;

namespace SsmsLite.Core.Ui.Text
{
    public class RunStream
    {
        public EventHandler<Run> OnMessage;

        public void SendRun(Run run)
        {
            OnMessage(this, run);
        }

        public void SendStandard(string text, bool newLine = true)
        {
            SendColored(text, Brushes.Black, newLine);
        }

        public void SendError(string text, bool newLine = true)
        {
            SendColored(text, Brushes.Red, newLine);
        }

        public void SendSuccess(string text, bool newLine = true)
        {
            SendColored(text, Brushes.ForestGreen, newLine);
        }

        public void SendWarning(string text, bool newLine = true)
        {
            SendColored(text, Brushes.DarkOrange, newLine);
        }

        public void SendColored(string text, Brush foreground, bool newLine = true)
        {
            if (newLine)
                text = Environment.NewLine + text;

            SendRun(RunExtensions.CreateColored(text, foreground));
        }

    }
}
