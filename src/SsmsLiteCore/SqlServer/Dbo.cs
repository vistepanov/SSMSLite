using System.Diagnostics;

namespace SsmsLite.Core.SqlServer
{
    [DebuggerDisplay("{ServerName} | {Database} | {Schema} | {Name}")]

    public class Dbo
    {
        public string ServerName { get; }
        public string Database { get; }
        public string Schema { get; }
        public string Name { get; set; }

        public Dbo(string[] obj)
        {
            ServerName = null;
            Database = null;
            Schema = null;
            Name = null;
            if (obj == null) return;

            Name = DeQuote(obj[0]);
            if (obj.Length > 1)
                Schema = DeQuote(obj[1]);
            if (obj.Length > 2)
                Database = DeQuote(obj[2]);
            if (obj.Length > 3)
                ServerName = DeQuote(obj[3]);
        }

        public override string ToString()
        {
            var rv = "";
            if (ServerName == null)
                rv += $"[{ServerName}]";
            if (Database == null)
                rv += $"[{Database}]";
            if (Schema == null)
                rv += $"[{Schema}]";
            if (Name == null)
                rv += $"[{Name}]";
            return rv;
        }

        private static string DeQuote(string val)
        {
            if (val == null) return null;

            if (val.StartsWith("["))
                return DecodeSb(val);
            if (val.StartsWith("\""))
                return DecodeQm(val);

            return val;
        }

        private static string DecodeSb(string val)
        {
            return val.Substring(1, val.Length - 2);
        }

        private static string DecodeQm(string val)
        {
            val = val.Replace("\"\"", "\"");
            return val.Substring(1, val.Length - 2);
        }
    }
}