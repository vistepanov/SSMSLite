using System;
using Microsoft.Extensions.Logging;
using SsmsLite.Core.Database;
using SsmsLite.Core.Database.Entities;
using SsmsLite.Core.Database.Entities.Persisted;

namespace SsmsLite.Db.DbUpdate
{
    public class DbUpdater
    {
        private readonly ILogger<DbUpdater> _logger;
        private readonly ILocalDatabase _db;


        public DbUpdater(
            ILogger<DbUpdater> logger, ILocalDatabase db
        )
        {
            _logger = logger;
            _db = db ?? throw new ArgumentException(nameof(db));
        }

        public void UpdateDb()
        {
            _db.Command(db =>
            {
                db.CreateIndex<QueryItem, DateTime>(o => o.ExecutionDateUtc);
                db.CreateIndex<QueryItem, string>(o => o.Database);
                db.CreateIndex<QueryItem, string>(o => o.Server);

                db.CreateIndex<AppVersion, int>(o => o.BuildNumber);

                db.CreateIndex<DbDefinition, string>(o => o.DbName);
                db.CreateIndex<DbDefinition, string>(o => o.Server);
                db.CreateIndex<DbDefinition, int>(o => o.DbId);

                db.CreateIndex<DbObject, int>(o => o.DbId);
                db.CreateIndex<DbColumn, int>(o => o.DbId);
                db.CreateIndex<DbIndex, int>(o => o.DbId);
                db.CreateIndex<DbIndexColumn, int>(o => o.DbId);

                return 0;
            });
        }

    }
}