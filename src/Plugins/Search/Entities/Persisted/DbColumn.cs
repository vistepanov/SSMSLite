namespace SsmsLite.Search.Entities.Persisted
{
    public class DbColumn: IDbId
    {
        public int DbId { get; set; }

        public long TableId { get; set; }

        public string Name { get; set; }

        public string Datatype { get; set; }

        public long? Precision { get; set; }

        public long? Scale { get; set; }

        public string Definition { get; set; }

        public DbObject Parent { get; set; }
    }
}