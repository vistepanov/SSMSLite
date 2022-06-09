using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SsmsLite.Core.App.Filtering;
using SsmsLite.Core.Database.Entities;
using SsmsLite.Core.Integration.Connection;

namespace SsmsLite.Core.Database
{
    public interface ILocalDatabase
    {
        T Command<T>(Func<ILocalDatabase, T> command, int timeout = 120);
        T Command<T>(Func<ILocalDatabase, T> command, Func<T> onOk, Func<Exception, T> onErr, int timeout = 120);
        void DropDb(int dbid);
        void InsertBulk<T>(IEnumerable<T> val);
        void InsertBulkInternal<T>(IEnumerable<T> val);
        QueryItem[] FindItems(FilterContext filterContext);

        int DbExists(DbConnectionString dbConnectionString);
        int GetNextDbId();
        void Insert<T>(T val);
        T[] FindByDbId<T>(int dbId) where T : IDbId;
        void CreateIndex<T, TK>(Expression<Func<T, TK>> keySelector);
        void Execute(string sql);
    }
}