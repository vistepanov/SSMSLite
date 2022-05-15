using SsmsLite.Core.Integration;

namespace SsmsLite.Core.Utils.TextProcessor.Actions
{
    public abstract class BaseCursorAction
    {
        public BaseCursorAction(PackageProvider addIn)
        {
            AddIn = addIn;
            // ImageIndex = AddIn.DefaultImageIndex;
            ShowWaitCursor = false;

        }

        public bool ShowWaitCursor { get; set; }

        protected internal PackageProvider AddIn { get; set; }

        protected static bool IsCapital(string line, int position)
        {
            if (position < 0)
                return true;

            return line[position] >= 'A' && line[position] <= 'Z';
        }

        protected static bool IsSpace(string rightOfCursor, int position)
        {
            if (position > rightOfCursor.Length)
                return false;

            return rightOfCursor[position] == ' ';
        }

        public bool CanExecute()
        {
            return AddIn.IsCurrentDocumentExtension("sql")
                   && AddIn.AllText.Length > 0;
        }
    }
}