using SsmsLite.Core.Integration;

namespace SsmsLite.Core.Utils.TextProcessor.Actions
{
    public abstract class BaseLeftCusorAction : BaseCursorAction
    {
        public BaseLeftCusorAction(PackageProvider addIn) : base(addIn)
        {
        }

        protected void CursorLeft(bool applySelection)
        {
            var textDocument = AddIn.TextDocument;
            var cursor = new Cursor(textDocument.Selection.CurrentColumn, textDocument.Selection.TopPoint.Line);

            if (cursor.Column == 1)
                return;

            var line = AddIn.CurrentLine;
            if (string.IsNullOrEmpty(line))
                return;

            var leftOfCursor = line.Substring(0, cursor.Column - 1);

            var position = leftOfCursor.Length - 1;
            while (position > 0)
            {
                if (IsSpace(leftOfCursor, position) && position != leftOfCursor.Length - 1)
                {
                    textDocument.Selection.CharLeft(applySelection, leftOfCursor.Length - position);
                    return;
                }

                if (IsCapital(leftOfCursor, position) && !IsCapital(leftOfCursor, position - 1))
                {
                    textDocument.Selection.CharLeft(applySelection, leftOfCursor.Length - position);

                    return;
                }

                position--;
            }

            textDocument.Selection.CharLeft(applySelection, leftOfCursor.Length);
        }
    }
}