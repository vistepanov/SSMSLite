using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SsmsLite.Core.Database.Entities.Persisted;
using SsmsLite.Core.Integration.Connection;
using SsmsLite.Core.SqlServer;

namespace SsmsLite.MsSqlDb
{
    public class SqlDbInfo
    {
        public async Task<DbObject[]> GetAllObjectsAsync(DbConnectionString dbConnectionString)
        {
            #region SQL

            const string dbObjectsQuery = @"
WITH DefinitionInfo (object_id, definition)  
AS
(
	SELECT m.object_id , m.definition from sys.sql_modules as m
	UNION
	SELECT c.object_id, 'CHECK ' + c.definition from sys.check_constraints as c

	UNION
	SELECT dc.object_id, 'DEFAULT' + dc.definition     + ' FOR [' + c.name + ']'
	FROM sys.default_constraints as dc
		INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
	UNION
	SELECT s.object_id, s.base_object_name from sys.synonyms  as s
)
SELECT o.object_id AS 'objectId' , o.type_desc as 'type', schema_name(o.schema_id) AS 'schemaName', o.name, DefinitionInfo.definition, o.modify_date as ModificationDate, Parent.object_id as parentObjectId
FROM sys.objects AS o
LEFT JOIN DefinitionInfo ON DefinitionInfo.object_id = o.object_id
LEFT JOIN sys.objects AS Parent ON Parent.object_id = o.parent_object_id and Parent.is_ms_shipped = 0
WHERE o.is_ms_shipped = 0 AND NOT (o.parent_object_id > 0 AND Parent.object_id IS NULL)";

            #endregion

            using (var connection = new SqlConnection(dbConnectionString.ConnectionString))
            {
                return (await connection.QueryAsync<DbObject>(dbObjectsQuery)).ToArray();
            }
        }

        public async Task<DbColumn[]> GetAllColumnsAsync(DbConnectionString dbConnectionString)
        {
            #region SQL

            const string columnsQuery = @"
SELECT c.id as 'TableId', c.name,  t.name AS 'datatype', c.prec AS 'precision', c.scale AS 'scale', cc.definition
FROM sys.syscolumns AS c
  INNER JOIN sys.objects AS o ON c.id = o.object_id
  INNER JOIN sys.types AS t ON c.xusertype = t.user_type_id
  LEFT  JOIN sys.computed_columns AS cc ON cc.object_id = o.object_id AND cc.column_id = c.colid
WHERE o.type = 'U' and o.is_ms_shipped = 0
ORDER BY c.id, c.colid";

            #endregion

            using (var connection = new SqlConnection(dbConnectionString.ConnectionString))
            {
                return (await connection.QueryAsync<DbColumn>(columnsQuery)).ToArray();
            }
        }

        public async Task<DbIndex[]> GetAllIndexesAsync(DbConnectionString dbConnectionString)
        {
            #region SQL

            const string indicesQuery = @"
SELECT t.object_id ownerId, i.index_id 'indexNumber', i.[name], i.type_desc 'type', i.filter_definition FilterDefinition, i.is_unique IsUnique
FROM sys.indexes i
  INNER JOIN sys.objects t ON t.object_id = i.object_id
WHERE t.is_ms_shipped = 0 AND index_id > 0
ORDER BY t.object_id, i.index_id";

            #endregion

            using (var connection = new SqlConnection(dbConnectionString.ConnectionString))
            {
                return (await connection.QueryAsync<DbIndex>(indicesQuery)).ToArray();
            }
        }

        public async Task<DbIndexColumn[]> GetAllIndexesColumnAsync(DbConnectionString dbConnectionString)
        {
            #region SQL

            const string indicesColumnsQuery = @"
SELECT OwnerId = ind.object_id,
	IndexNumber = ind.index_id,
	IndexColumnId = ic.index_column_id,
	OwnerColumnId = col.column_id,
	ColumnName = col.name,
	IsDesc = ic.is_descending_key,
    Included = ic.is_included_column
FROM sys.indexes ind 
  INNER JOIN sys.index_columns ic	ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
  INNER JOIN sys.columns col			ON ic.object_id = col.object_id and ic.column_id = col.column_id 
  INNER JOIN sys.objects t			ON ind.object_id = t.object_id 
WHERE  
	t.is_ms_shipped = 0 
ORDER BY 
    ind.object_id, ind.index_id, ic.index_column_id;";

            #endregion

            using (var connection = new SqlConnection(dbConnectionString.ConnectionString))
            {
                return (await connection.QueryAsync<DbIndexColumn>(indicesColumnsQuery)).ToArray();
            }
        }

        public async Task<DbObject> GetObjectByName(DbConnectionString dbConnectionString, Dbo dbo)
        {
            #region SQL

            const string dbObjectsQueryFiltered = @"
WITH DefinitionInfo (object_id, definition)
AS
(
  SELECT m.object_id , m.definition from sys.sql_modules as m
  UNION

  SELECT c.object_id, 'CHECK ' + c.definition from sys.check_constraints as c
  UNION

  SELECT dc.object_id, 'DEFAULT' + dc.definition     + ' FOR [' + c.name + ']'
  FROM sys.default_constraints as dc
    INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
  UNION

  SELECT s.object_id, s.base_object_name from sys.synonyms  as s
)
SELECT o.object_id AS 'objectId' , o.type_desc as 'type', schema_name(o.schema_id) AS 'schemaName', o.name, DefinitionInfo.definition, o.modify_date as ModificationDate, Parent.object_id as parentObjectId
FROM sys.objects AS o
  LEFT JOIN DefinitionInfo ON DefinitionInfo.object_id = o.object_id
  LEFT JOIN sys.objects AS Parent ON Parent.object_id = o.parent_object_id and Parent.is_ms_shipped = 0
WHERE o.is_ms_shipped = 0
  AND NOT (o.parent_object_id > 0 AND Parent.object_id IS NULL)
  AND o.name=@name
  AND (schema_name(o.schema_id)=@schemaName OR ISNULL(@schemaName, '')='')";

            #endregion

            var dbc = new DbConnectionString(dbConnectionString, dbo.Database);
            using (var connection = new SqlConnection(dbc.ConnectionString))
            {
                var rv = await connection
                    .QueryFirstOrDefaultAsync<DbObject>(dbObjectsQueryFiltered
                        , new
                        {
                            name = dbo.Name,
                            schemaName = dbo.Schema
                        });
                return rv;
            }
        }
    }
}