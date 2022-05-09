using System;
using System.IO;
using LiteDB;
using Microsoft.Extensions.Logging;
using SsmsLite.Core.App;

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
            var fileName = Path.Combine(workingDirProvider.GetWorkingDir(), "SSMS-Plus.lite");
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
            return;
        }

        public void Dispose()
        {
            _database?.Dispose();
        }
    }
}