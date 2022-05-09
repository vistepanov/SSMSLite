using SSMSPlusSearch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SsmsLite.Core.Database;
using SsmsLite.Core.Integration.Connection;
using SsmsLite.Core.Utils;
using SSMSPlusSearch.Entities.Persisted;
using SSMSPlusSearch.Entities.Search;

namespace SSMSPlusSearch.Repositories
{
    public class SchemaSearchRepository
    {
        private readonly Db _db;
        private readonly ILogger<SchemaSearchRepository> _logger;

        public SchemaSearchRepository(ILogger<SchemaSearchRepository> logger, Db db)
        {
            _logger = logger;
            _db = db ?? throw new ArgumentException(nameof(db));
        }

        public void DropDb(int dbid)
        {
            _db.Command((db) =>
            {
                db.GetCollection<DbIndexColumn>()
                    .DeleteMany(p => p.DbId == dbid);
                db.GetCollection<DbIndex>()
                    .DeleteMany(p => p.DbId == dbid);
                db.GetCollection<DbColumn>()
                    .DeleteMany(p => p.DbId == dbid);
                db.GetCollection<DbObject>()
                    .DeleteMany(p => p.DbId == dbid);
                db.GetCollection<DbDefinition>()
                    .Delete(dbid);
                return 0;
            });
        }

        public int DbExists(DbConnectionString dbConnectionString)
        {
            return _db.GetCollection<DbDefinition>().Query()
                .Where(p => p.DbName.Equals(dbConnectionString.Database)
                                    && p.Server.Equals(dbConnectionString.Server))
                .FirstOrDefault()?
                .DbId??0;
        }

        public int InsertDb(DbDefinition dbDefinition, DbObject[] dbObjects, DbColumn[] columns,
            DbIndex[] indices, DbIndexColumn[] indicesColumns)
        {
            return _db.Command((db) =>
            {
                var dbDef = db.GetCollection<DbDefinition>();
                var dbId = 1;
                if (dbDef.Count()>0) 
                    dbId = dbDef.Max(t=> t.DbId) + 1;

                dbDefinition.DbId = dbId;
                dbObjects.ForEach(p => p.DbId = dbId);
                columns.ForEach(p => p.DbId = dbId);
                indices.ForEach(p => p.DbId = dbId);
                indicesColumns.ForEach(p => p.DbId = dbId);

                dbDef.Insert(dbDefinition);
                db.GetCollection<DbObject>().InsertBulk(dbObjects);
                db.GetCollection<DbColumn>().InsertBulk(columns);
                db.GetCollection<DbIndex>().InsertBulk(indices);
                db.GetCollection<DbIndexColumn>().InsertBulk(indicesColumns);

                return dbId;
            });
        }

        public ISearchTarget[] GetObjectsByDb(int dbid)
        {
            var dbObjects = FindByDbId<DbObject>(dbid);
            var dbColumns = FindByDbId<DbColumn>(dbid);
            var dbIndices = FindByDbId<DbIndex>(dbid);
            var dbIndicesColumns = FindByDbId<DbIndexColumn>(dbid);

            var dbObjectById = dbObjects.ToDictionary(p => p.ObjectId);
            MapDbObjectParents(dbObjectById);
            MapDbColumnsParents(dbObjectById, dbColumns);
            MapDbIndicesParents(dbObjectById, dbIndices);
            MapIndicesColumns(dbIndices, dbIndicesColumns);

            var dbColumnsByTableId = dbColumns.ToLookup(p => p.TableId);
            var dbObjectsTargets = CreateObjectBasedSearchTarget(dbObjects, dbColumnsByTableId);
            var dbColumnsTargets = dbColumns.Select(p => new ColumnSearchTarget(p));
            var dbIndicesTargets = dbIndices.Select(p => new IndexSearchTarget(p)).ToArray();

            var list = new List<ISearchTarget>();
            list.AddRange(dbObjectsTargets);
            list.AddRange(dbColumnsTargets);
            list.AddRange(dbIndicesTargets);
            return list.OrderByDescending(p => p.ModificationDateStr).ToArray();
        }

        private IEnumerable<ISearchTarget> CreateObjectBasedSearchTarget(DbObject[] dbObjects,
            ILookup<long, DbColumn> dbColumnsByTableId)
        {
            var list = new List<ISearchTarget>();
            foreach (var dbObject in dbObjects)
            {
                var simplifiedType = DbObjectType.Parse(dbObject.Type);

                if (simplifiedType.Category == DbSimplifiedType.Constraint)
                {
                    list.Add(new ConstraintSearchTarget(dbObject));
                }
                else if (simplifiedType.Category == DbSimplifiedType.Table)
                {
                    list.Add(new TableSearchTarget(dbObject, dbColumnsByTableId[dbObject.ObjectId].ToArray()));
                }
                else if (simplifiedType.Category == DbSimplifiedType.Other)
                {
                    list.Add(new OtherSearchTarget(dbObject));
                }
                else list.Add(new ObjectSearchTarget(dbObject));
            }

            return list;
        }

        private static void MapIndicesColumns(DbIndex[] dbIndices, DbIndexColumn[] dbIndicesColumns)
        {
            var columnsByIndexId = dbIndicesColumns.ToLookup(p => ValueTuple.Create(p.OwnerId, p.IndexNumber));
            foreach (var index in dbIndices)
            {
                index.Columns = columnsByIndexId[ValueTuple.Create(index.OwnerId, index.IndexNumber)].ToArray();
            }
        }

        private static void MapDbObjectParents(Dictionary<long, DbObject> dbObjectById)
        {
            foreach (var obj in dbObjectById.Values.Where(obj => obj.ParentObjectId != null
                                                                 && obj.ParentObjectId.HasValue))
            {
                if (obj.ParentObjectId != null) obj.Parent = dbObjectById[obj.ParentObjectId.Value];
            }
        }

        private static void MapDbColumnsParents(Dictionary<long, DbObject> dbObjectById, DbColumn[] dbColumns)
        {
            foreach (var column in dbColumns)
            {
                column.Parent = dbObjectById[column.TableId];
            }
        }

        private static void MapDbIndicesParents(Dictionary<long, DbObject> dbObjectById, DbIndex[] dbIndices)
        {
            foreach (var index in dbIndices)
            {
                index.Parent = dbObjectById[index.OwnerId];
            }
        }

        private T[] FindByDbId<T>(int dbId) where T : IDbId
        {
            var collection = _db.GetCollection<T>();
            collection.EnsureIndex(t => t.DbId);
            return collection.Query().Where(t => t.DbId == dbId).ToArray();
        }
    }
}