using EnvDTE;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.UI.VSIntegration;
using Microsoft.VisualStudio.Shell;
using SSMSPlusHistory.Entities;
using SSMSPlusHistory.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using SsmsLite.Core.Integration;
using SsmsLite.Core.Integration.Clipboard;
using Task = System.Threading.Tasks.Task;

namespace SSMSPlusHistory.Services
{
    public class QueryTracker : IDisposable
    {
        private readonly ConcurrentQueue<QueryItem> _itemsQueue = new ConcurrentQueue<QueryItem>();

        private readonly ILogger<QueryTracker> _logger;
        private readonly QueryItemRepository _queryItemRepository;
        private readonly PackageProvider _packageProvider;

        private bool _isTracking;

        public CommandEvents QueryExecuteEvent { get; private set; }

        public QueryTracker(PackageProvider packageProvider, ILogger<QueryTracker> logger,
            QueryItemRepository queryItemRepository)
        {
            _logger = logger;
            _queryItemRepository = queryItemRepository;
            _packageProvider = packageProvider;
        }

        public void StartTracking()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_isTracking)
            {
                throw new Exception("HistoryPlugin is already registered");
            }

            _isTracking = true;
            var dte2 = _packageProvider.Dte2;
            var command = dte2.Commands.Item("Query.Execute");

            QueryExecuteEvent = dte2.Events.CommandEvents[command.Guid, command.ID];
//            QueryExecuteEvent = dte2.Events.get_CommandEvents(command.Guid, command.ID);
            QueryExecuteEvent.BeforeExecute += CommandEvents_BeforeExecute;
        }

        /// <summary>
        /// Handle event before exec sql script
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="id"></param>
        /// <param name="customIn"></param>
        /// <param name="customOut"></param>
        /// <param name="cancelDefault"></param>
        private void CommandEvents_BeforeExecute(string guid, int id, object customIn, object customOut,
            ref bool cancelDefault)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var queryText = SqlDocument.GetQueryText(_packageProvider);

                if (string.IsNullOrWhiteSpace(queryText))
                    return;

                // Get Current Connection Information
                var connInfo = ServiceCache.ScriptFactory.CurrentlyActiveWndConnectionInfo.UIConnectionInfo;

                var queryItem = new QueryItem
                {
                    Query = queryText,
                    Server = connInfo.ServerName,
                    Username = connInfo.UserName,
                    Database = connInfo.AdvancedOptions["DATABASE"],
                    ExecutionDateUtc = DateTime.UtcNow
                };

                _logger.LogInformation("Enqueued {@queryItem}", queryItem.Query);
                lock (_itemsQueue)
                {
                    _itemsQueue.Enqueue(queryItem);
                }

                Task.Delay(1000).ContinueWith(t => this.SavePendingItems(), TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on BeforeExecute tracking");
            }
        }

        /// <summary>
        /// write text to db
        /// </summary>
        private void SavePendingItems()
        {
            var pendingItems = new List<QueryItem>();
            lock (_itemsQueue)
            {
                while (_itemsQueue.TryDequeue(out var queryItem))
                {
                    pendingItems.Add(queryItem);
                }
            }

            _queryItemRepository.Insert(pendingItems);
        }

        public void Dispose()
        {
            SavePendingItems();
        }
    }
}