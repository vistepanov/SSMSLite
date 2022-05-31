using System;
using System.Collections.Generic;
using System.Linq;
using SsmsLite.Search.Repositories.Search;

namespace SsmsLite.Search.Services.Filtering
{
    public class FilterResultService
    {
        private static readonly Dictionary<MatchOn, Func<ISearchTarget, string, bool>> MatchPredicates
            = new Dictionary<MatchOn, Func<ISearchTarget, string, bool>>();

        static FilterResultService()
        {
            MatchPredicates.Add(MatchOn.Name, (p, str) => p.MatchsName(str));
            MatchPredicates.Add(MatchOn.Definition, (p, str) => p.MatchsDefinition(str));
        }

        public static IEnumerable<ISearchTarget> Filter(IEnumerable<ISearchTarget> source, FilterContext filterContext)
        {
            source = source.Where(p => filterContext.Schemas.Contains(p.SchemaName));
            source = source.Where(p => filterContext.Categories.Contains(p.TypeCategory));

            if (!string.IsNullOrEmpty(filterContext.Search))
            {
                if (filterContext.MatchesOn.Count == 0)
                    return Enumerable.Empty<ISearchTarget>();

                Func<ISearchTarget, string, bool> predicate = BuildMatchOnPredicate(filterContext.MatchesOn);
                source = source.Where(p => predicate(p, filterContext.Search));
            }

            return source;
        }

        private static Func<ISearchTarget, string, bool> BuildMatchOnPredicate(HashSet<MatchOn> matchOns)
        {
            var predicates = matchOns.Select(p => MatchPredicates[p]).ToArray();

            bool Predicate(ISearchTarget r, string str)
            {
                var orResult = false;
                foreach (var predicate in predicates)
                {
                    orResult = predicate(r, str) || orResult;
                }

                return orResult;
            }

            return Predicate;
        }
    }
}