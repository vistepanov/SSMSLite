using System;

namespace SSMSPlusSearch.Entities.Persisted
{
    public class DbDefinition : IDbId
    {
        public int DbId { get; set; }

        public string Server { get; set; }

        public string DbName { get; set; }

        public DateTime IndexDateUtc { get; set; }
    }
}
