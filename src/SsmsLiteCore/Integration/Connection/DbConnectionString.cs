using System;
using System.Data.SqlClient;

namespace SsmsLite.Core.Integration.Connection
{
    public class DbConnectionString : IEquatable<DbConnectionString>
    {
        public string ConnectionString { get; }
        public string Database { get; }
        public string Server { get; }

        public string DisplayName => Database + " @ " + Server;

        public DbConnectionString(DbConnectionString dbConnection, string database = null)
        {
            var builder = new SqlConnectionStringBuilder(dbConnection.ConnectionString);
            if (database != null)
            {
                builder.InitialCatalog = database;
            }

            ConnectionString = builder.ToString();
            Database = builder.InitialCatalog;
            Server = builder.DataSource;
        }

        public DbConnectionString(string connectionString, string database = null)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            if (database != null)
            {
                builder.InitialCatalog = database;
            }

            ConnectionString = builder.ToString();
            Database = builder.InitialCatalog;
            Server = builder.DataSource;
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 23 + (Server == null ? 0 : Server.GetHashCode());
            hash = hash * 23 + (Database == null ? 0 : Database.GetHashCode());
            return hash;
        }

        public bool Equals(DbConnectionString other)
        {
            return string.Equals(Server, other?.Server, StringComparison.InvariantCultureIgnoreCase)
                   &&
                   string.Equals(Database, other?.Database, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DbConnectionString cnx))
                return false;

            return Equals(cnx);
        }
    }
}