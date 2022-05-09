using System;

namespace SsmsLite.Core.Database.Entities
{
    public class AppVersion
    {
        public int BuildVersion { get; set; }
        public int BuildNumber { get; set; }
        public DateTime Utc { get; set; }
    }

}