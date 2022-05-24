
using System;
using System.Threading.Tasks;
using System.Windows;
using EnvDTE;
using Microsoft.Extensions.Logging;
using SsmsLite.Core.Integration;
using SsmsLite.Core.Integration.Connection;
using SsmsLite.Core.Integration.ObjectExplorer;
using SsmsLite.MsSqlDb;
using SsmsLite.Core.Database;

namespace SsmsLite.Sync
{
    public class Sync
    {
        public const int MenuCommandId = 0x0203;
        private bool _isRegistered;
        private readonly PackageProvider _packageProvider;
        private readonly IObjectExplorerInteraction _objectExploreInteraction;
        private readonly SqlDbInfo _dbInfo;
        private readonly DbConnectionProvider _dbConnectionProvider;
        private readonly ILogger<Sync> _logger;

        public Sync(PackageProvider packageProvider
            , DbConnectionProvider dbConnectionProvider
            , IObjectExplorerInteraction objectExploreInteraction
            , ILogger<Sync> logger
            , SqlDbInfo dbInfo)
        {
            _packageProvider = packageProvider;
            _objectExploreInteraction = objectExploreInteraction;
            // _db = db ?? throw new ArgumentException(nameof(db));
            _dbInfo = dbInfo;
            _logger = logger;
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
        /// Limitation - simple parsing, can not understand Delimited identifiers with '[' inside, 
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void MenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                string world;
                var parser = new SqlServerParser(_packageProvider);
                world = parser.FindCurrentObject();
                if (string.IsNullOrWhiteSpace(world))
                    world = _packageProvider.CurrentWord;

                await FindText(world);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sync failed");
            }
        }

        private async Task<bool> FindText(string world)
        {
            var document = _packageProvider.GetTextDocument();
            //  if (document == null) return true;

            try
            {
                var dbConnectionString = _dbConnectionProvider.GetFromActiveConnection();
                if (dbConnectionString == null) return true;
                var dbObject = await _dbInfo.GetObjectByName(dbConnectionString, world);
                var path = dbObject?.DbRelativePath();
                if (path == null) return true;
                await _objectExploreInteraction.SelectNodeAsync(dbConnectionString.Server, dbConnectionString.Database,
                    path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot find {world}, {ex.Message}");
            }

            document?.Selection.Cancel();
            return false;
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