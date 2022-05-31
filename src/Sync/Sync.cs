using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Logging;
using SsmsLite.Core.Integration;
using SsmsLite.Core.Integration.Connection;
using SsmsLite.Core.Integration.ObjectExplorer;
using SsmsLite.MsSqlDb;
using SsmsLite.Core.Database;
using SsmsLite.Core.SqlServer;

namespace SsmsLite.Sync
{
    public class Sync
    {
        public const int MenuCommandId = 1401;
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
                var parser = new SqlServerParser(_packageProvider);
                var dbo = new Dbo(parser.FindCurrentObject());
                if (string.IsNullOrWhiteSpace(dbo.Name))
                    dbo.Name = _packageProvider.CurrentWord;

                await SelectDbo(dbo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sync failed");
            }
        }

        private async Task SelectDbo(Dbo dbo)
        {
            try
            {
                var dbConnectionString = _dbConnectionProvider.GetFromActiveConnection(
                    !string.IsNullOrEmpty(dbo.Database), dbo
                );
                if (dbConnectionString == null) return;
                var dbObject = await _dbInfo.GetObjectByName(dbConnectionString, dbo);

                var path = dbObject?.DbRelativePath();
                if (path == null) return;
                await _objectExploreInteraction
                    .SelectNodeAsync(dbConnectionString.Server, dbConnectionString.Database, path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot find {dbo}, {ex.Message}");
            }
        }
    }
}