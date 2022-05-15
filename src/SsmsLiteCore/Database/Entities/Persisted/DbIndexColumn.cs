namespace SsmsLite.Core.Database.Entities.Persisted
{
    public class DbIndexColumn : IDbId
    {
        public int DbId { get; set; }
        public int OwnerId { get; set; }
        public int IndexNumber { get; set; }
        public int IndexColumnId { get; set; }
        public int OwnerColumnId { get; set; }
        public string ColumnName { get; set; }
        public bool Included { get; set; }
        public bool IsDesc { get; set; }
    }
}