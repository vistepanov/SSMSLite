namespace SsmsLite.Core.Database.Entities.Persisted
{
    public class DbIndex : IDbId
    {
        public int DbId { get; set; }
        public int OwnerId { get; set; }
        public int IndexNumber { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string FilterDefinition { get; set; }
        public bool IsUnique { get; set; }

        public DbObject Parent { get; set; }
        public DbIndexColumn[] Columns { get; set; }
    }
}
