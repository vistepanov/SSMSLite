using System;
using System.Threading.Tasks;
using SsmsLite.Core.Integration.Connection;
using SsmsLite.Core.SqlServer;
using SsmsLite.Core.Utils;
using SsmsLite.MsSqlDb;
using SsmsLite.Core.Database.Entities.Persisted;
using SsmsLite.Search.Repositories;

namespace SsmsLite.Search.Services
{

    public class DbIndexer : IDbIndexer
    {
        private readonly SchemaSearchRepository _schemaSearchRepository;

        private readonly AsyncLock _asyncLock = new AsyncLock();
        private readonly SqlDbInfo _dbInfo;

        public DbIndexer(SchemaSearchRepository schemaSearchRepository, SqlDbInfo dbInfo)
        {
            _schemaSearchRepository = schemaSearchRepository;
            _dbInfo = dbInfo;
        }

        public async Task<int> ReIndexAsync(DbConnectionString dbConnectionString)
        {
            using (await _asyncLock.LockAsync())
            {
                var dbid = _schemaSearchRepository.DbExists(dbConnectionString);
                if (dbid > 0)
                {
                    _schemaSearchRepository.DropDb(dbid);
                }

                return await IndexAsync(dbConnectionString);
            }
        }

        public int DbExists(DbConnectionString dbConnectionString)
        {
            return _schemaSearchRepository.DbExists(dbConnectionString);
        }

        public async Task<int> IndexAsync(DbConnectionString dbConnectionString)
        {
            if (DbHelper.IsSystemDb(dbConnectionString.Database))
            {
                throw new Exception("Cannot index system db: " + dbConnectionString.Database);
            }

            using (await _asyncLock.LockAsync())
            {
                var dbId = DbExists(dbConnectionString);
                if (dbId > 0)
                    return dbId;

                var dbObjects = await _dbInfo.GetAllObjectsAsync(dbConnectionString);
                var dbColumns = await _dbInfo.GetAllColumnsAsync(dbConnectionString);
                var dbIndices = await _dbInfo.GetAllIndexesAsync(dbConnectionString);
                var dbIndicesColumns = await _dbInfo.GetAllIndexesColumnAsync(dbConnectionString);

                var dbDefinition = new DbDefinition()
                {
                    DbName = dbConnectionString.Database,
                    Server = dbConnectionString.Server,
                    IndexDateUtc = DateTime.UtcNow
                };

                return _schemaSearchRepository.InsertDb(dbDefinition, dbObjects, dbColumns, dbIndices, dbIndicesColumns);
            }
        }
    }
}
