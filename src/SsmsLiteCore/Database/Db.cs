using System;
using System.Collections.Generic;
using System.IO;
using LiteDB;
using Microsoft.Extensions.Logging;
using SsmsLite.Core.App;
using SsmsLite.Core.App.Filtering;
using SsmsLite.Core.Database.Entities;

namespace SsmsLite.Core.Database
{
    public class Db : IDisposable
    {
        private readonly string _connectionString;
        private readonly LiteDatabase _database;
        private readonly ILogger<Db> _logger;

        public Db(ILogger<Db> logger, IWorkingDirProvider workingDirProvider)
        {
            _logger = logger;
            var fileName = Path.Combine(workingDirProvider.GetWorkingDir(), "SSMS.lite");
            _connectionString = $"Filename={fileName};Upgrade=true;";
            _database = GetDatabase();
        }

        public LiteDatabase GetDatabase()
        {
            return new LiteDatabase(_connectionString);
        }

        public T Command<T>(Func<Db, T> command, int timeout = 120) => Command(command, null, null, timeout);

        public T Command<T>(Func<Db, T> command, Func<T> onOk, Func<Exception, T> onErr, int timeout = 120)
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

        public ILiteCollection<T> GetCollection<T>(string cName = null)
        {
            return _database.GetCollection<T>(cName ?? typeof(T).Name);
        }

        public void Execute(string sql)
        {
            _database.Execute(sql);
        }

        public void Dispose()
        {
            _database?.Dispose();
        }

        public void DropDb(int dbid)
        {
            Command((db) =>
            {
                _database.Execute($"DELETE DbIndexColumn WHERE $.DbId={dbid};");
                _database.Execute($"DELETE DbIndex WHERE $.DbId={dbid};");
                _database.Execute($"DELETE DbColumn WHERE $.DbId={dbid};");
                _database.Execute($"DELETE DbObject WHERE $.DbId={dbid};");
                _database.Execute($"DELETE DbDefinition WHERE $.DbId={dbid};");
                return 0;
            });
        }

        public int InsertBulk<T>(IEnumerable<T> val)
        {
            return _database.GetCollection<T>().InsertBulk(val);
        }

        public QueryItem[] FindItems(FilterContext filterContext)
        {
            var query = _database.GetCollection<QueryItem>()
                .Query().Where(t => t.ExecutionDateUtc >= filterContext.FromUtc && t.ExecutionDateUtc <= filterContext.ToUtc);

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
}