using SsmsLite.Core.App.Filtering;
using SsmsLite.Core.Ui.Search;
using SsmsLite.History.Entities.Search;

namespace SsmsLite.History.UI
{
    public class SearchFilterResultVM
    {
        public SearchFilterResultVM(ScriptSearchTarget searchResult, FilterContext filterContext)
        {
            SearchResult = searchResult;
            FilterContext = filterContext;
        }

        public ScriptSearchTarget SearchResult { get; }
        public FilterContext FilterContext { get; }

        public TextFragments ServerHighlight => SearchResult.ServerHighlight(FilterContext.ServerSearch);

        public TextFragments DatabaseHighlight => SearchResult.DatabaseHighlight(FilterContext.DbSearch);

        public TextFragments QueryHighlight => SearchResult.QueryHighlight(FilterContext.QuerySearch);

        public TextFragments SmallQueryHighlight => SearchResult.SmallQueryHighlight(FilterContext.QuerySearch);
    }
}