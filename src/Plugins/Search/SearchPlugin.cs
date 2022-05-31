using System;

namespace SsmsLite.Search
{
    public class SearchPlugin
    {
        private bool _isRegistered;
        private readonly SearchUi _searchUi;

        public SearchPlugin(SearchUi searchUi)
        {
            _searchUi = searchUi;
        }

        public void Register()
        {
            if (_isRegistered)
            {
                throw new Exception("SearchPlugin is already registered");
            }

            _isRegistered = true;
            _searchUi.Register();
        }
    }
}