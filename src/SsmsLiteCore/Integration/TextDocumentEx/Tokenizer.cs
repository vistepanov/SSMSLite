using EnvDTE;
using SsmsLite.Core.Utils.TextProcessor;

namespace SsmsLite.Core.Integration.TextDocumentEx
{
    public class Tokenizer
    {
        private bool _foundBracket = false;
        private TextDocument _document;

        public Tokenizer(TextDocument document)
        {
            _document = document;
        }


        private EditPoint Left(EditPoint start, bool stopOnSpace)
        {
            var point = start.CreateEditPoint();
            do
            {
                point.CharLeft(1);
                var leftChar = point.GetText(start)[0];
                if (point.AtStartOfDocument || leftChar == '.' || leftChar == '[' || (leftChar == ' ' && stopOnSpace))
                {
                    return point;
                }
            } while (true);
        }

        public Cursor Cursor
        {
            get
            {
                var point = _document.Selection.TopPoint;
                return new Cursor(point.DisplayColumn, point.Line);
            }
            set
            {
                var point = _document.Selection.TopPoint.CreateEditPoint();
                point.LineDown(value.Row);
                point.CharRight(value.Column);
                _document.Selection.MoveToPoint(point, false);
            }
        }

    }
}