using EnvDTE;
using Microsoft.SqlServer.Management.SqlParser.Parser;

namespace SsmsLite.Sync
{
    public static class LocationsHelper
    {
        public static bool Contain(Location current, Location start, Location end)
        {
            return current.LineNumber >= start.LineNumber
                   && (current.ColumnNumber >= start.ColumnNumber
                       || current.LineNumber > start.LineNumber
                   )
                   && end.LineNumber >= current.LineNumber
                   && (end.ColumnNumber >= current.ColumnNumber
                       || end.LineNumber > current.LineNumber);
        }

        public static Location CurrentLocation(TextDocument doc)
        {
            var ap = doc.Selection.ActivePoint;
            return new Location(ap.Line, ap.LineCharOffset);
        }
    }
}