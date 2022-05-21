using System;
using System.Windows;
using EnvDTE;
using Microsoft.Extensions.Logging;
using SsmsLite.Core.Database;
using SsmsLite.Core.Integration;
using SsmsLite.Core.Integration.Connection;
using SsmsLite.Core.Integration.ObjectExplorer;
using SsmsLite.MsSqlDb;

namespace SsmsLite.Sync
{
    public class Sync
    {
        public const int MenuCommandId = 0x0203;
        private bool _isRegistered;
        private readonly PackageProvider _packageProvider;
        private readonly ILogger<Sync> _logger;

        private readonly IObjectExplorerInteraction _objectExploreInteraction;

        // private readonly Db _db;
        private readonly SqlDbInfo _dbInfo;
        private readonly DbConnectionProvider _dbConnectionProvider;

        public Sync(PackageProvider packageProvider
            , ILogger<Sync> logger
            , DbConnectionProvider dbConnectionProvider
            , IObjectExplorerInteraction objectExploreInteraction
            // , Db db
            , SqlDbInfo dbInfo)
        {
            _packageProvider = packageProvider;
            _logger = logger;
            _objectExploreInteraction = objectExploreInteraction;
            // _db = db ?? throw new ArgumentException(nameof(db));
            _dbInfo = dbInfo;
            _dbConnectionProvider = dbConnectionProvider;
        }

        public void Register()
        {
            if (_isRegistered)
            {
                throw new Exception("Sync is already registered");
            }

            _isRegistered = true;

            MenuHelper.AddMenuCommand(_packageProvider, MenuItemCallback, MenuCommandId);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void MenuItemCallback(object sender, EventArgs e)
        {
            var document = _packageProvider.GetTextDocument();
            if (document == null) return;
            var world = _packageProvider.CurrentWord;

            try
            {
                var dbConnectionString = _dbConnectionProvider.GetFromActiveConnection();
                if (dbConnectionString == null) return;
                var dbObject = await _dbInfo.GetObjectByName(dbConnectionString, world);
                var path = dbObject?.DbRelativePath();
                if (path == null) return;
// так, тут разобраться с с itemPath или сделать свой селектНод
                await _objectExploreInteraction.SelectNodeAsync(dbConnectionString.Server, dbConnectionString.Database,
                    path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot find {world}, {ex.Message}");
            }

            document.Selection.Cancel();
        }

        private string LookupObject(TextSelection point)
        {
            if (point.IsEmpty)
            {
                point.WordLeft();
                point.WordRight(true);
            }


            return point.Text;
        }
    }
}