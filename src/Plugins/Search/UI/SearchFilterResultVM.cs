using SsmsLite.Core.Ui.Search;
using SsmsLite.Search.Repositories.Search;
using SsmsLite.Search.Services.Filtering;

namespace SsmsLite.Search.UI
{
    public class SearchFilterResultVM
    {
        public ISearchTarget SearchResult { get; }
        public FilterContext FilterContext { get; }

        public SearchFilterResultVM(ISearchTarget searchResult, FilterContext filterContext)
        {
            this.SearchResult = searchResult;
            this.FilterContext = filterContext;
        }

        public TextFragments NameHighlight
        {
            get
            {
                if (FilterContext.MatchesOn.Contains(MatchOn.Name))
                {
                    return SearchResult.RichNameHighlight(FilterContext.Search);
                }
                else
                {
                    return SearchResult.RichName;
                }
            }
        }

        public TextFragments DefinitionHighlight
        {
            get
            {
                if (FilterContext.MatchesOn.Contains(MatchOn.Definition))
                {
                    return SearchResult.RichSmallDefinitionHighlight(FilterContext.Search);
                }
                else
                {
                    return SearchResult.RichSmallDefinition;
                }
            }
        }

        public TextFragments FullPreviewHighlight
        {
            get
            {
                if (FilterContext.MatchesOn.Contains(MatchOn.Definition))
                {
                    return SearchResult.RichFullDefinitionHighlight(FilterContext.Search);
                }
                else
                {
                    return SearchResult.RichFullDefinition;
                }
            }
        }
    }
}