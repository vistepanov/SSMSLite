﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SsmsLite.Core.Database.Entities;
using SsmsLite.Core.Integration.Connection;
using SsmsLite.Core.Integration.ObjectExplorer;
using SsmsLite.Core.Ui;
using SsmsLite.Core.Ui.Commands;
using SsmsLite.Core.Ui.Controls.ComboCheckBox;
using SsmsLite.Core.Utils;
using SsmsLite.Search.Repositories;
using SsmsLite.Search.Repositories.Search;
using SsmsLite.Search.Services;
using SsmsLite.Search.Services.Filtering;

namespace SsmsLite.Search.UI
{
    public class SchemaSearchControlVm : ViewModelBase
    {
        private readonly IDbIndexer _dbIndexer;
        private readonly SchemaSearchRepository _schemaSearchRepository;
        private readonly IObjectExplorerInteraction _objectExploreInteraction;

        private DbConnectionString _dbConnectionString;
        private int _dbid;
        private ISearchTarget[] _allDdResults;


        public IAsyncCommand ReIndexDbCmd { get; }
        public IAsyncCommand ExecuteSearchCmd { get; }
        public IAsyncCommand<SearchFilterResultVM> LocateItemCmd { get; }
        public Command<SearchFilterResultVM> CopyItemNameCmd { get; }
        public Command<SearchFilterResultVM> CopyItemDefinitionCmd { get; }

        public ComboCheckBoxViewModel<MatchOn> ComboMatchVM { get; private set; }
        public ComboCheckBoxViewModel<DbSimplifiedType> ComboObjectsVM { get; private set; }
        public ComboCheckBoxViewModel<string> SchemaObjectsVM { get; }

        public SchemaSearchControlVm(IDbIndexer dbIndexer, SchemaSearchRepository schemaSearchRepository,
            IObjectExplorerInteraction objectExploreInteraction)
        {
            _objectExploreInteraction = objectExploreInteraction;
            _dbIndexer = dbIndexer;
            _schemaSearchRepository = schemaSearchRepository;
            ReIndexDbCmd = new AsyncCommand(OnReIndexDbAsync, CanExecuteSubmit, HandleError);
            ExecuteSearchCmd = new AsyncCommand(FuncHelper.Debounce(ExecuteSearchAsync, 100), CanExecuteSubmit,
                HandleError);
            LocateItemCmd = new AsyncCommand<SearchFilterResultVM>(LocateAsync, (_) => true, HandleError);
            CopyItemNameCmd = new Command<SearchFilterResultVM>(OnCopyItemName, null, HandleError);
            CopyItemDefinitionCmd = new Command<SearchFilterResultVM>(OnCopyItemDefinition, null, HandleError);

            CreateMatchOnCombo();
            CreateObjectsCombo();
            SchemaObjectsVM = new ComboCheckBoxViewModel<string>();
        }

        #region Context Menu

        private async Task LocateAsync(SearchFilterResultVM item)
        {
            var itemPath = item.SearchResult.DbRealtivePath();
            if (itemPath.Count == 0)
            {
                Message = "Sorry, no locate implemented for the type :" + item.SearchResult.SqlObjectType;
                return;
            }

            await _objectExploreInteraction.SelectNodeAsync(_dbConnectionString.Server, _dbConnectionString.Database,
                itemPath);
        }

        private void OnCopyItemName(SearchFilterResultVM item)
        {
            Clipboard.SetText(item.SearchResult.Name);
        }

        private void OnCopyItemDefinition(SearchFilterResultVM item)
        {
            Clipboard.SetText(item.SearchResult.RichFullDefinition.AsString);
        }

        #endregion

        private void CreateObjectsCombo()
        {
            ComboObjectsVM = new ComboCheckBoxViewModel<DbSimplifiedType>();
            foreach (var objectTypeCategory in DbSimplifiedType.GetAll())
            {
                ComboObjectsVM.Items.Add(new ComboCheckBoxItem<DbSimplifiedType>()
                    { Text = objectTypeCategory.Name, IsChecked = true, Value = objectTypeCategory });
            }

            ComboObjectsVM.SelectionChanged += ComboVM_SelectionChanged;
        }

        private void CreateMatchOnCombo()
        {
            ComboMatchVM = new ComboCheckBoxViewModel<MatchOn>
            {
                IsAllVisible = false
            };
            ComboMatchVM.Items.Add(new ComboCheckBoxItem<MatchOn>()
                { Text = "Name", IsChecked = true, Value = MatchOn.Name });
            ComboMatchVM.Items.Add(new ComboCheckBoxItem<MatchOn>()
                { Text = "Definition", IsChecked = true, Value = MatchOn.Definition });
            ComboMatchVM.SelectionChanged += ComboVM_SelectionChanged;
        }

        private void ReCreateSchemaCombo(IEnumerable<string> schemas)
        {
            SchemaObjectsVM.Items.Clear();

            foreach (var schema in schemas)
            {
                SchemaObjectsVM.Items.Add(new ComboCheckBoxItem<string>()
                    { Text = schema, IsChecked = true, Value = schema });
            }

            SchemaObjectsVM.SelectionChanged -= ComboVM_SelectionChanged;
            SchemaObjectsVM.SelectionChanged += ComboVM_SelectionChanged;
        }

        public async Task InitializeDbAsync(DbConnectionString cnxStr)
        {
            _dbConnectionString = cnxStr;
            DbDisplayName = _dbConnectionString.DisplayName;
            await SafeTask.RunSafeAsync(InitializeDbAsync, HandleError);
        }

        private async Task InitializeDbAsync()
        {
            var dbid = _dbIndexer.DbExists(_dbConnectionString);
            if (dbid == 0)
            {
                dbid = await IndexDbAsync();
            }

            InitializeDbResults(dbid);
            await ExecuteSearchAsync();
        }

        private async Task OnReIndexDbAsync()
        {
            IsIndexing = true;
            Message = "";

            var dbId = await _dbIndexer.ReIndexAsync(_dbConnectionString);
            InitializeDbResults(dbId);
            await ExecuteSearchAsync();

            IsIndexing = false;
        }

        private async Task<int> IndexDbAsync()
        {
            IsIndexing = true;
            Message = "";

            var dbId = await _dbIndexer.IndexAsync(_dbConnectionString);

            IsIndexing = false;
            return dbId;
        }

        private void InitializeDbResults(int dbId)
        {
            _dbid = dbId;
            _allDdResults = _schemaSearchRepository.GetObjectsByDb(_dbid);
            ReCreateSchemaCombo(_allDdResults.Select(p => p.SchemaName).Distinct().OrderBy(p => p));
        }

        private async Task ExecuteSearchAsync()
        {
            try
            {
                IsLoading = true;

                var sw = System.Diagnostics.Stopwatch.StartNew();

                await Task.Run(() =>
                {
                    var filter = new FilterContext(
                        Filter
                        , ComboMatchVM.GetSelectedValues()
                        , ComboObjectsVM.GetSelectedValues()
                        , SchemaObjectsVM.GetSelectedValues()
                    );
                    SearchResultsVM = FilterResultService
                        .Filter(_allDdResults, filter)
                        .Select(p => new SearchFilterResultVM(p, filter))
                        .Take(100)
                        .ToList();
                }).ConfigureAwait(false);

                Message = $"{SearchResultsVM.Count} Result(s) in {sw.ElapsedMilliseconds} ms";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanExecuteSubmit()
        {
            return !IsIndexing;
        }

        private void HandleError(Exception ex)
        {
            Message = ex.GetFullStackTraceWithMessage();
        }

        private string _filter;

        public string Filter
        {
            get => _filter;
            set
            {
                SetField(ref _filter, value);
                ExecuteSearchCmd.ExecuteAsync();
            }
        }

        private string _message = "Click on the button to load data";

        public string Message
        {
            get => _message;
            set => SetField(ref _message, value);
        }

        private string _dbDisplayName;

        public string DbDisplayName
        {
            get => _dbDisplayName;
            set => SetField(ref _dbDisplayName, value);
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetField(ref _isLoading, value);
        }

        private bool _isIndexing;

        public bool IsIndexing
        {
            get => _isIndexing;
            set
            {
                SetField(ref _isIndexing, value);
                RaisePropertyChanged(nameof(ControlsEnabled));
            }
        }

        public bool ControlsEnabled => !IsIndexing;

        private SearchFilterResultVM _selectedItem;

        public SearchFilterResultVM SelectedItem
        {
            get => _selectedItem;
            set => SetField(ref _selectedItem, value);
        }

        private List<SearchFilterResultVM> _searchResultsVM;

        public List<SearchFilterResultVM> SearchResultsVM
        {
            get => _searchResultsVM;
            private set => SetField(ref _searchResultsVM, value);
        }

        private void ComboVM_SelectionChanged(object sender, EventArgs e)
        {
            ExecuteSearchCmd.ExecuteAsync();
        }

        public void Free()
        {
            _searchResultsVM?.Clear();
            _searchResultsVM = null;
            _allDdResults = null;
        }
    }
}