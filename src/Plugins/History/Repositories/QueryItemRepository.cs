using System;
using System.Collections.Generic;
using SsmsLite.Core.App.Filtering;
using SsmsLite.Core.Database;
using SsmsLite.Core.Database.Entities;

namespace SsmsLite.History.Repositories
{
    public class QueryItemRepository
    {
        private readonly ILocalDatabase _db;

        public QueryItemRepository(ILocalDatabase db)
        {
            _db = db ?? throw new ArgumentException(nameof(db));
        }

        public void Insert(IEnumerable<QueryItem> queryItems)
        {
            _db.InsertBulk(queryItems);
        }

        public QueryItem[] FindItems(FilterContext filterContext)
        {
            return _db.FindItems(filterContext);
        }
    }
}