using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SsmsLite.Core.Database.Entities
{
    public class DbSimplifiedType
    {
        public static readonly DbSimplifiedType Table = new DbSimplifiedType("Table");
        public static readonly DbSimplifiedType Column = new DbSimplifiedType("Column");
        public static readonly DbSimplifiedType View = new DbSimplifiedType("View");
        public static readonly DbSimplifiedType Procedure = new DbSimplifiedType("Procedure");
        public static readonly DbSimplifiedType Function = new DbSimplifiedType("Function");
        public static readonly DbSimplifiedType Trigger = new DbSimplifiedType("Trigger");
        public static readonly DbSimplifiedType Constraint = new DbSimplifiedType("Constraint");
        public static readonly DbSimplifiedType Index = new DbSimplifiedType("Index");
        public static readonly DbSimplifiedType Other = new DbSimplifiedType("Other");

        public string Name { get; }

        private DbSimplifiedType(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IEnumerable<DbSimplifiedType> GetAll()
        {
            var fields = typeof(DbSimplifiedType).GetFields(BindingFlags.Public |
                                                            BindingFlags.Static |
                                                            BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<DbSimplifiedType>();
        }
    }
}