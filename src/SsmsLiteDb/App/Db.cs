using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using LiteDB;
using Microsoft.Extensions.Logging;
using SsmsLite.Core.App;
using SsmsLite.Core.App.Filtering;
using SsmsLite.Core.Database;
using SsmsLite.Core.Database.Entities;
using SsmsLite.Core.Database.Entities.Persisted;
using SsmsLite.Core.Integration.Connection;

namespace SsmsLite.Db.App
{
    public class Db : ILocalDatabase
    {
        private readonly string _connectionString;
        private LiteDatabase _database;
        private readonly ILogger<Db> _logger;

        public Db(ILogger<Db> logger, IWorkingDirProvider workingDirProvider)
        {
            _logger = logger;
            var fileName = Path.Combine(workingDirProvider.GetWorkingDir(), "SSMS.lite");
            _connectionString = $"Filename={fileName};Upgrade=true;";
            _database = null;
        }

        public T Command<T>(Func<ILocalDatabase, T> command, int timeout = 120) =>
            Command(command, null, null, timeout);

        public T Command<T>(Func<ILocalDatabase, T> command, Func<T> onOk, Func<Exception, T> onErr,
            int timeout = 120)
        {
            using (_database = new LiteDatabase(_connectionString))
            {
                _database.Timeout = TimeSpan.FromSeconds(timeout);
                _database.BeginTrans();

                try
                {
                    var result = command(this);
                    _database.Commit();
                    return onOk != null ? onOk() : result;
                }
                catch (Exception ex)
                {
                    _database.Rollback();
                    if (onErr != null)
                    {
                        return onErr(ex);
                    }

                    throw;
                }
            }
        }

        // public ILiteCollection<T> GetCollection<T>(string cName = null)
        // {
        //     if (_database == null) throw new ArgumentNullException(nameof(_database));
        //     return _database.GetCollection<T>(cName ?? typeof(T).Name);
        // }

        public void Execute(string sql)
        {
            if (_database == null) throw new ArgumentNullException(nameof(_database));
            _database.Execute(sql);
        }

        public void DropDb(int dbid)
        {
            _logger.LogDebug($"Drop info for db {dbid}");
            Command((db) =>
            {
                db.Execute($"DELETE DbIndexColumn WHERE $.DbId={dbid};");
                db.Execute($"DELETE DbIndex WHERE $.DbId={dbid};");
                db.Execute($"DELETE DbColumn WHERE $.DbId={dbid};");
                db.Execute($"DELETE DbObject WHERE $.DbId={dbid};");
                db.Execute($"DELETE DbDefinition WHERE $.DbId={dbid};");
                return 0;
            });
        }

        public void InsertBulk<T>(IEnumerable<T> val)
        {
            using (_database = new LiteDatabase(_connectionString))
            {
                InsertBulkInternal(val);
            }
        }

        public void InsertBulkInternal<T>(IEnumerable<T> val)
        {
            _database.GetCollection<T>().InsertBulk(val);
        }

        public QueryItem[] FindItems(FilterContext filterContext)
        {
            using (_database = new LiteDatabase(_connectionString))
            {
                var query = _database.GetCollection<QueryItem>()
                    .Query().Where(t =>
                        t.ExecutionDateUtc >= filterContext.FromUtc && t.ExecutionDateUtc <= filterContext.ToUtc);

                if (!string.IsNullOrEmpty(filterContext.QuerySearch))
                    query = query.Where(t => t.Query.Contains(filterContext.QuerySearch));

                if (!string.IsNullOrEmpty(filterContext.DbSearch))
                    query = query.Where(t => t.Database.Contains(filterContext.DbSearch));

                if (!string.IsNullOrEmpty(filterContext.ServerSearch))
                    query = query.Where(t => t.Server.Contains(filterContext.ServerSearch));

                return query.OrderByDescending(t => t.Id)
                    .Limit(1000)
                    .ToArray();
            }
        }

        public int DbExists(DbConnectionString dbConnectionString)
        {
            using (_database = new LiteDatabase(_connectionString))
            {
                return _database.GetCollection<DbDefinition>()
                    .Query()
                    .Where(p => p.DbName.Equals(dbConnectionString.Database)
                                && p.Server.Equals(dbConnectionString.Server))
                    .FirstOrDefault()?
                    .DbId ?? 0;
            }
        }

        public int GetNextDbId()
        {
            if (_database == null) throw new ArgumentNullException(nameof(_database));

            var dbDef = _database.GetCollection<DbDefinition>();
            if (dbDef.Count() == 0) return 1;
            return dbDef.Max(t => t.DbId) + 1;
        }

        public void Insert<T>(T val)
        {
            if (_database == null) throw new ArgumentNullException(nameof(_database));
            _database.GetCollection<T>().Insert(val);
        }

        public T[] FindByDbId<T>(int dbId) where T : IDbId
        {
            var collection = _database.GetCollection<T>();
            collection.EnsureIndex(t => t.DbId);
            return collection.Query().Where(t => t.DbId == dbId).ToArray();
        }

        public void CreateIndex<T, TK>(Expression<Func<T, TK>> keySelector)
        {
            _database.GetCollection<T>().EnsureIndex(keySelector);
        }
    }
}