using System;

namespace SsmsLite.Core.App.Filtering
{
    public class FilterContext
    {
        public string QuerySearch { get; }
        public string ServerSearch { get; }
        public string DbSearch { get; }
        public DateTime FromUtc { get; }
        public DateTime ToUtc { get; }

        public FilterContext(string querySearch, string serverSearch, string dbSearch, DateTime fromUtc, DateTime toUtc)
        {
            QuerySearch = querySearch;
            ServerSearch = serverSearch;
            DbSearch = dbSearch;
            FromUtc = fromUtc;
            ToUtc = toUtc;
        }
    }
}
