using System.Collections.Generic;
using SsmsLite.Core.Database.Entities;

namespace SsmsLite.Search.Services.Filtering
{
    public class FilterContext
    {
        public string Search { get; }
        public HashSet<MatchOn> MatchesOn { get; }
        public HashSet<DbSimplifiedType> Categories { get; }
        public HashSet<string> Schemas { get; }

        public FilterContext(string search, HashSet<MatchOn> matchesOn, HashSet<DbSimplifiedType> categories,
            HashSet<string> schemas)
        {
            this.Search = search;
            this.MatchesOn = matchesOn;
            this.Categories = categories;
            this.Schemas = schemas;
        }
    }
}