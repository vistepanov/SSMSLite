using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using SsmsLite.Core.App;
using SsmsLite.Core.Database.Entities;
using SsmsLite.History.Entities;
using SsmsLite.Search.Entities.Persisted;

namespace SsmsLite.Db.DbUpdate
{
    public class DbUpdater
    {
        private readonly IVersionProvider _versionProvider;
        private readonly Assembly _resourcesAssembly;
        private readonly ILogger<DbUpdater> _logger;
        private readonly Core.Database.Db _db;


        public DbUpdater(
            ILogger<DbUpdater> logger
            , IVersionProvider versionProvider
            , Core.Database.Db db
        )
        {
            _logger = logger;
            _versionProvider = versionProvider;
            _resourcesAssembly = typeof(DbUpdater).Assembly;
            _db = db ?? throw new ArgumentException(nameof(db));
        }

        public void UpdateDb()
        {
            _db.GetCollection<QueryItem>().EnsureIndex(o => o.ExecutionDateUtc);
            _db.GetCollection<QueryItem>().EnsureIndex(o => o.Database);
            _db.GetCollection<QueryItem>().EnsureIndex(o => o.Server);

            _db.GetCollection<AppVersion>().EnsureIndex(o => o.BuildNumber);

            _db.GetCollection<DbDefinition>().EnsureIndex(o => o.DbName);
            _db.GetCollection<DbDefinition>().EnsureIndex(o => o.Server);
            _db.GetCollection<DbDefinition>().EnsureIndex(o => o.DbId);

            _db.GetCollection<DbObject>().EnsureIndex(o => o.DbId);
            _db.GetCollection<DbColumn>().EnsureIndex(o => o.DbId);
            _db.GetCollection<DbIndex>().EnsureIndex(o => o.DbId);
            _db.GetCollection<DbIndexColumn>().EnsureIndex(o => o.DbId);
        }

    }
}