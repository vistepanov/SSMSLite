using System.Diagnostics;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace SsmsLite.Core.Utils.TextProcessor
{
    [DebuggerDisplay("Col: {Column} | Row: {Row}")]
    public class Cursor
    {
        public int Column { get; set; }
        public int Row { get; set; }
        public Cursor(int col, int row)
        {
            Column = col;
            Row = row;
        }
        public override bool Equals(object obj)
        {
            var other = obj as Cursor;
            if (other == null)
                return false;

            return other.Row == Row && other.Column == Column;
        }

        public override int GetHashCode()
        {
            return Row.GetHashCode()
                   ^ Column.GetHashCode();
        }

    }
}