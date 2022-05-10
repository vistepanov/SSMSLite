using System;
using System.Collections.Generic;
using SsmsLite.Core.Database;
using SsmsLite.History.Entities;
using SsmsLite.History.Services.Filtering;

namespace SsmsLite.History.Repositories
{
    public class QueryItemRepository
    {
        private readonly Db _db;

        public QueryItemRepository(Db db)
        {
            _db = db ?? throw new ArgumentException(nameof(db));
        }

        public void Insert(IEnumerable<QueryItem> queryItems)
        {
            _db.GetCollection<QueryItem>().InsertBulk(queryItems);
        }

        public QueryItem[] FindItems(FilterContext filterContext)
        {
            var query = _db.GetCollection<QueryItem>().Query()
                .Where(t => t.ExecutionDateUtc >= filterContext.FromUtc && t.ExecutionDateUtc <= filterContext.ToUtc);

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