using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using SsmsLite.Core.Utils.TextProcessor;

namespace SsmsLite.Core.Integration.TextDocumentEx
{
    /// <summary>
    /// table_name
    ///     Is the name of table. Table names must follow the rules for [identifiers].
    /// identifier
    ///     identifier can be a maximum of 128 characters, except for local temporary table names that cannot exceed 116 characters.
    ///     https://docs.microsoft.com/en-us/sql/relational-databases/databases/database-identifiers?view=sql-server-2016
    /// Regular Identifiers
    ///     The first character must be one of the following:
    ///         A letter as defined by the Unicode Standard 3.2. The Unicode definition of letters includes Latin characters from a through z, from A through Z, and also letter characters from other languages.
    ///         The underscore(_), at sign(@), or number sign(#).
    /// 
    /// </summary>
    public class Tokenizer
    {
        private bool _foundBracket = false;
        private TextDocument _document;
        private VirtualPoint _ap;
        private List<string> _list;

        public Tokenizer(TextDocument document)
        {
            _document = document;
            _ap = _document.Selection.ActivePoint;
            _list = new List<string>();
        }

        public void findObjectId()
        {
            var dlmStart = new[] { '[', '\"', ' ' };
            var dlm = new[] { '[', ']', '\"', ' ', '\n' };
            var cursor = _ap.CreateEditPoint();
            var allLeft = _ap.CreateEditPoint();
            allLeft.CharLeft(130);
            var text = allLeft.GetText(cursor).Trim();
            var lastChar = text.Last();
            if (lastChar == ']' || lastChar == '\"')
                text = text.PadLeft(text.Length - 1);
            if (text.Contains("[") || text.Contains("\""))
            {
                // жопная хрень
                for (int i = text.Length - 1; i >= 0; i--)
                {
                    if (dlmStart.Contains(text[i]))
                    {
                        _list.Add(text.Substring(i));
                    }
                }
            }

            foreach (var str in _list)
            {
                var lastIndex = str.LastIndexOfAny(new[] { ']', '\"' });
                if (lastIndex >= 0)
                {
                }
            }
            // left 128 + 2 char, look for '"', '[' and ']' 
            // left until found '.' or '[' or '"' or ' '
            // right until found '.' or ']' or '"' or ' '
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
                _document.Selection.MoveToPoint(point);
            }
        }

        public string AllText()
        {
            var point = _document.StartPoint.CreateEditPoint();
            point.StartOfDocument();
            return point.GetText(_document.EndPoint);
        }

        public void Parse()
        {
            //Microsoft.SqlServer.Management.
        }
    }
}