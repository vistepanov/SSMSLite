using System;
using SsmsLite.Core.Integration;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace SsmsLite.Result
{
    public class ResultSelectionTracker
    {
        private bool _isTracking;
        private readonly PackageProvider _packageProvider;

        public CommandEvents QueryExecuteEvent { get; private set; }

        public ResultSelectionTracker(PackageProvider packageProvider)
        {
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
//            var command = dte2.Commands.Item("Query.Execute");
            dte2.Events.SelectionEvents.OnChange += OnEventExec;

//            QueryExecuteEvent = dte2.Events.CommandEvents[command.Guid, command.ID];
//            QueryExecuteEvent.BeforeExecute += OnEventExec;
        }

        /// <summary>
        /// https://docs.microsoft.com/ru-ru/dotnet/api/envdte.selecteditems?view=visualstudiosdk-2017
        /// </summary>
        private void OnEventExec()
        {
            var selectedItems = _packageProvider.Dte2.SelectedItems;
            if (selectedItems == null || !selectedItems.MultiSelect) return;
            var topDte = selectedItems.DTE;
            var parent = selectedItems.Parent;
            var container = selectedItems.SelectionContainer;
            //parent.ActiveDocument.Type
        }
    }
}