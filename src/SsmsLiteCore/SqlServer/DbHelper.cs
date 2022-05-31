using System.Linq;

namespace SsmsLite.Core.SqlServer
{
    public static class DbHelper
    {
        private static readonly string[] SystemDbs = { "master", "model", "msdb", "tempdb" };

        public static bool IsSystemDb(string db)
        {
            return SystemDbs.Contains(db.ToLower());
        }

    }
}