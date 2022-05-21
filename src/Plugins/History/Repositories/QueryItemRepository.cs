using System;
using System.Collections.Generic;
using SsmsLite.Core.App.Filtering;
using SsmsLite.Core.Database;
using SsmsLite.Core.Database.Entities;

namespace SsmsLite.History.Repositories
{
    public class QueryItemRepository
    {
        private readonly Db _db;

        public QueryItemRepository(Db db)
        {
            _db = db ?? throw new ArgumentException(nameof(db));
        }

        public int Insert(IEnumerable<QueryItem> queryItems)
        {
            return _db.InsertBulk(queryItems);
        }

        public QueryItem[] FindItems(FilterContext filterContext)
        {
            return _db.FindItems(filterContext);
        }
    }
}