using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using SsmsLite.Core.App;
using SsmsLite.Core.Database;
using SsmsLite.Core.Database.Entities;
using SsmsLite.Core.Database.Entities.Persisted;

namespace SsmsLite.Db.DbUpdate
{
    public class DbUpdater
    {
        private readonly IVersionProvider _versionProvider;
        private readonly Assembly _resourcesAssembly;
        private readonly ILogger<DbUpdater> _logger;
        private readonly ILocalDatabase _db;


        public DbUpdater(
            ILogger<DbUpdater> logger
            , IVersionProvider versionProvider
            , ILocalDatabase db
        )
        {
            _logger = logger;
            _versionProvider = versionProvider;
            _resourcesAssembly = typeof(DbUpdater).Assembly;
            _db = db ?? throw new ArgumentException(nameof(db));
        }

        public void UpdateDb()
        {
            _db.CreateIndex<QueryItem, DateTime>(o => o.ExecutionDateUtc);
            _db.CreateIndex<QueryItem, string>(o => o.Database);
            _db.CreateIndex<QueryItem, string>(o => o.Server);

            _db.CreateIndex<AppVersion, int>(o => o.BuildNumber);

            _db.CreateIndex<DbDefinition, string>(o => o.DbName);
            _db.CreateIndex<DbDefinition, string>(o => o.Server);
            _db.CreateIndex<DbDefinition, int>(o => o.DbId);

            _db.CreateIndex<DbObject, int>(o => o.DbId);
            _db.CreateIndex<DbColumn, int>(o => o.DbId);
            _db.CreateIndex<DbIndex, int>(o => o.DbId);
            _db.CreateIndex<DbIndexColumn, int>(o => o.DbId);
        }

    }
}