namespace SsmsLite.Core.Database.Entities
{
    public class DbToken
    {
        public string Database { get; set; }
        public string Schema { get; set; }
        public string Name { get; set; }

        public DbToken(string name, string schema = null, string dbName = null)
        {
            Database = dbName;
            Schema = schema;
            Name = name;
        }
    }
}