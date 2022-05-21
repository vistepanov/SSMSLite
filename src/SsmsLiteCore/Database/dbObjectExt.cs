using System.Collections.Generic;
using SsmsLite.Core.Database.Entities.Persisted;

namespace SsmsLite.Core.Database
{
    public static class DbObjectExt
    {
        //public DbObjectType SqlObjectType => _sqlObjectType ?? (_sqlObjectType = DbObjectType.Parse(Type));
        public static IReadOnlyCollection<string> DbRelativePath(this DbObject dbObject)
        {
            switch (dbObject.Type)
            {
                case "AGGREGATE_FUNCTION":
                    return dbObject.ObjPath("UserProgrammability", "UsrDbFunctions", "AggregateFunctions");
                case "CHECK_CONSTRAINT":
                    return dbObject.TableParentPath("Constraints");
                case "CLR_SCALAR_FUNCTION": return null; // Do not show it
                case "CLR_TABLE_VALUED_FUNCTION": return null; // Do not show it
                case "DEFAULT_CONSTRAINT":
                    return dbObject.TableParentPath("Constraints");
                case "FOREIGN_KEY_CONSTRAINT":
                    return dbObject.TableParentPath("Keys");
                case "INTERNAL_TABLE": return null; // Do not show it
                case "PRIMARY_KEY_CONSTRAINT":
                    return dbObject.TableParentPath("Keys");
                case "SERVICE_QUEUE":
                    return dbObject.ObjPath("ServiceBroker", "Queues");
                case "SQL_INLINE_TABLE_VALUED_FUNCTION":
                    return dbObject.ObjPath("UserProgrammability", "UsrDbFunctions", "Table-valuedFunctions");
                case "SQL_SCALAR_FUNCTION":
                    return dbObject.ObjPath("UserProgrammability", "UsrDbFunctions", "Scalar-valuedFunctions");
                case "SQL_STORED_PROCEDURE":
                    return dbObject.ObjPath("UserProgrammability", "StoredProcedures");
                case "SQL_TABLE_VALUED_FUNCTION":
                    return dbObject.ObjPath("UserProgrammability", "UsrDbFunctions", "Table-valuedFunctions");
                case "SQL_TRIGGER":
                    return dbObject.TableParentPath("Triggers");
                case "SYNONYM":
                    return dbObject.ObjPath("Synonyms");
                case "SYSTEM_TABLE": return null; // Do not show it
                //return dbObject.ObjPath("UserTables", "SystemTables");
                case "TYPE_TABLE": return null; // Do not show it
                case "UNIQUE_CONSTRAINT":
                    return dbObject.TableParentPath("Constraints");
                case "USER_TABLE":
                    return dbObject.ObjPath("UserTables");
                case "VIEW":
                    return dbObject.ObjPath("Views");
                default:
                    return null; //new string[] { };
            }

            //var isCheckOrDefault = dbObject.SqlObjectType == DbObjectType.CHECK_CONSTRAINT || this.SqlObjectType == DbObjectType.DEFAULT_CONSTRAINT;
            //var folder = isCheckOrDefault ? "Constraints" : "Keys";
        }

        private static IReadOnlyCollection<string> ObjPath(this DbObject dbObject, string objectType)
        {
            return new List<string>() { objectType, ObjId(dbObject) };
        }

        private static IReadOnlyCollection<string> ObjPath(this DbObject dbObject, string objectType,
            string objectSubType)
        {
            return new List<string>() { objectType, objectSubType, ObjId(dbObject) };
        }

        private static IReadOnlyCollection<string> ObjPath(this DbObject dbObject, string objectType,
            string objectSubType, string SubSubType)
        {
            return new List<string>() { objectType, objectSubType, SubSubType, ObjId(dbObject) };
        }

        private static IReadOnlyCollection<string> TableParentPath(this DbObject dbObject, string objectType)
        {
            var parent = dbObject.Parent;
            return new List<string>() { "UserTables", ObjId(parent), objectType, dbObject.Name };
        }

        private static string ObjId(DbObject dbObject) => $"{dbObject.SchemaName}.{dbObject.Name}";
    }
}