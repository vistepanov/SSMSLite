using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SsmsLite.Core.App;
using SsmsLite.Core.Integration;
using SsmsLite.Core.Ui;
using SsmsLite.Core.Ui.Commands;
using SsmsLite.Core.Utils;
using SsmsLite.History.Entities.Search;
using SsmsLite.History.Repositories;
using SsmsLite.History.Services.Filtering;

namespace SsmsLite.History.UI
{
    public class HistoryControlVm : ViewModelBase
    {
        private QueryItemRepository _itemsRepository;
        private IVersionProvider _versionProvider;
        private IServiceCacheIntegration _serviceCacheIntegration;

        public IAsyncCommand RequestItemsCommand { get; private set; }
        public IAsyncCommand ViewLoadedCommand { get; private set; }
        public Command<SearchFilterResultVM> OpenScriptCmd { get; }

        private bool _loadedOnce = false;

        public HistoryControlVm(QueryItemRepository itemsRepository, IVersionProvider versionProvider,
            IServiceCacheIntegration serviceCacheIntegration)
        {
            _itemsRepository = itemsRepository;
            _versionProvider = versionProvider;
            _serviceCacheIntegration = serviceCacheIntegration;
            RequestItemsCommand = new AsyncCommand(FuncHelper.Debounce(ExecuteRequestItemsAsync, 100), CanExecuteSubmit, HandleError);
            ViewLoadedCommand = new AsyncCommand(OnViewLoadedAsync, CanExecuteSubmit, HandleError);
            OpenScriptCmd = new Command<SearchFilterResultVM>(OpenScript, () => true, HandleError);
            InitDefaults();
        }

        private void OpenScript(SearchFilterResultVM arg)
        {
            _serviceCacheIntegration.OpenScriptInNewWindow(arg.SearchResult.QueryItem.Query);
        }

        private bool CanExecuteSubmit()
        {
            return !IsLoading;
        }

        private async Task OnViewLoadedAsync()
        {
            if (_loadedOnce)
                return;

            await RequestItemsCommand.ExecuteAsync();
            _loadedOnce = true;
        }

        private Task ExecuteRequestItemsAsync()
        {
            try
            {
                IsLoading = true;

                var sw = System.Diagnostics.Stopwatch.StartNew();

                var filterContext = new FilterContext(QueryFilter, ServerFilter, DbFilter, StartDate.ToUniversalTime(), EndDate.ToUniversalTime());
                var result = _itemsRepository.FindItems(filterContext);
                QueryItemsVM = result.Select(q => new ScriptSearchTarget(q))
                    .Select(p => new SearchFilterResultVM(p, filterContext)).ToList();

                Message = $"{QueryItemsVM.Count} Result(s) in {sw.ElapsedMilliseconds} ms";
                return Task.CompletedTask;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void HandleError(Exception ex)
        {
            Message = ex.Message;
        }

        private void InitDefaults()
        {
            _endDate = DateTime.Now.AddDays(1).Date;
            _startDate = EndDate.AddDays(-60).Date;
        }

        private List<SearchFilterResultVM> _queryItemsVM;

        public List<SearchFilterResultVM> QueryItemsVM
        {
            get => _queryItemsVM;
            private set => SetField(ref _queryItemsVM, value);
        }

        private string _message = "Click on the button to load data";

        public string Message
        {
            get => _message;
            set => SetField(ref _message, value);
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetField(ref _isLoading, value);
        }

        private DateTime _startDate;

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                SetField(ref _startDate, value);
                OnSearchChange();
            }
        }

        private DateTime _endDate;

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                SetField(ref _endDate, value);
                OnSearchChange();
            }
        }

        private string _queryFilter;

        public string QueryFilter
        {
            get => _queryFilter;
            set
            {
                SetField(ref _queryFilter, value);
                OnSearchChange();
            }
        }

        private string _dbFilter;

        public string DbFilter
        {
            get => _dbFilter;
            set
            {
                SetField(ref _dbFilter, value);
                OnSearchChange();
            }
        }

        private string _serverFilter;

        public string ServerFilter
        {
            get => _serverFilter;
            set
            {
                SetField(ref _serverFilter, value);
                OnSearchChange();
            }
        }

        private SearchFilterResultVM _selectedItem;

        public SearchFilterResultVM SelectedItem
        {
            get => _selectedItem;
            set { SetField(ref _selectedItem, value); }
        }

        private void OnSearchChange()
        {
            RequestItemsCommand.ExecuteAsync();
        }
    }
}