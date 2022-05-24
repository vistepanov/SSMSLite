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
        private LiteDatabase database;
        private readonly ILogger<Db> _logger;

        public Db(ILogger<Db> logger, IWorkingDirProvider workingDirProvider)
        {
            _logger = logger;
            var fileName = Path.Combine(workingDirProvider.GetWorkingDir(), "SSMS.lite");
            _connectionString = $"Filename={fileName};Upgrade=true;";
            database = null;
        }

        public T Command<T>(Func<ILocalDatabase, T> command, int timeout = 120) =>
            Command(command, null, null, timeout);

        public T Command<T>(Func<ILocalDatabase, T> command, Func<T> onOk, Func<Exception, T> onErr,
            int timeout = 120)
        {
            using (database = new LiteDatabase(_connectionString))
            {
                database.Timeout = TimeSpan.FromSeconds(timeout);
                database.BeginTrans();

                try
                {
                    var result = command(this);
                    database.Commit();
                    return onOk != null ? onOk() : result;
                }
                catch (Exception ex)
                {
                    database.Rollback();
                    if (onErr != null)
                    {
                        return onErr(ex);
                    }

                    throw;
                }
            }
        }

        public ILiteCollection<T> GetCollection<T>(string cName = null)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            return database.GetCollection<T>(cName ?? typeof(T).Name);
        }

        public void Execute(string sql)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            database.Execute(sql);
        }

        public void DropDb(int dbid)
        {
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

        public int InsertBulk<T>(IEnumerable<T> val)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            return database.GetCollection<T>().InsertBulk(val);
        }

        public QueryItem[] FindItems(FilterContext filterContext)
        {
            using (database = new LiteDatabase(_connectionString))
            {
                var query = database.GetCollection<QueryItem>()
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
            using (database = new LiteDatabase(_connectionString))
            {
                return database.GetCollection<DbDefinition>()
                    .Query()
                    .Where(p => p.DbName.Equals(dbConnectionString.Database)
                                && p.Server.Equals(dbConnectionString.Server))
                    .FirstOrDefault()?
                    .DbId ?? 0;
            }
        }

        public int GetNextDbId()
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            var dbDef = database.GetCollection<DbDefinition>();
            if (dbDef.Count() == 0) return 1;
            return dbDef.Max(t => t.DbId) + 1;
        }

        public void Insert<T>(T val)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            database.GetCollection<T>().Insert(val);
        }

        public T[] FindByDbId<T>(int dbId) where T : IDbId
        {
            var collection = database.GetCollection<T>();
            collection.EnsureIndex(t => t.DbId);
            return collection.Query().Where(t => t.DbId == dbId).ToArray();
        }

        public void CreateIndex<T, TK>(Expression<Func<T, TK>> keySelector)
        {
            database.GetCollection<T>().EnsureIndex(keySelector);
        }
    }
}