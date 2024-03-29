﻿using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo.RegSvrEnum;
using Microsoft.SqlServer.Management.UI.VSIntegration;
using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
using Microsoft.VisualStudio.Shell;
using SsmsLite.Core.Integration.ObjectExplorer;
using SsmsLite.Core.SqlServer;

namespace SsmsLite.Core.Integration.Connection
{
    public class DbConnectionProvider
    {
        private readonly PackageProvider _packageProvider;

        public DbConnectionProvider(PackageProvider packageProvider)
        {
            _packageProvider = packageProvider;
        }

        /// <summary>
        /// Return DbConnectionString to active window
        /// </summary>
        /// <returns>DbConnectionString</returns>
        public DbConnectionString GetFromActiveConnection(bool showSystemDb=false, Dbo dbo = null)
        {
            var connectionInfo = ServiceCache.ScriptFactory.CurrentlyActiveWndConnectionInfo?.UIConnectionInfo;
            
            var database = dbo?.Database ?? connectionInfo?.AdvancedOptions["DATABASE"];

            if (connectionInfo == null || database == null )
                return null;
            if (!showSystemDb && DbHelper.IsSystemDb(database))
                return null;

            return new DbConnectionString(GetConnectionString(connectionInfo), database);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>DbConnectionString</returns>
        public DbConnectionString GetFromSelectedDatabase()
        {
            return ThreadHelper.JoinableTaskFactory.Run(async () =>
                {
                    var objectExplorerService = await _packageProvider.AsyncPackage
                        .GetServiceAsync(typeof(IObjectExplorerService)) as IObjectExplorerService;
                    var dbNode = ObjectExplorerHelper.FindSelectedDatabaseNode(objectExplorerService);
                    return dbNode == null
                        ? null
                        : new DbConnectionString(dbNode.Connection.ConnectionString, dbNode.InvariantName);
                }
            );
        }

        private static string GetConnectionString(UIConnectionInfo connection)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = connection.ServerName,
                IntegratedSecurity = (connection.AuthenticationType == 0), // string.IsNullOrEmpty(connection.Password),
                Password = connection.Password,
                UserID = connection.UserName,
                InitialCatalog = connection.AdvancedOptions["DATABASE"],
                ApplicationName = "SsmsLite"
            };

            return builder.ToString();
        }
    }
}