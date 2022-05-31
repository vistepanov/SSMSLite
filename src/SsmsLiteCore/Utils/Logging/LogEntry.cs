using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace SsmsLite.Core.Utils.Logging
{
    public class LogEntry
    {
        public LogEntry()
        {
            TimeStampUtc = DateTime.UtcNow;
        }

        //public string UserName { get; private set; }
        public DateTime TimeStampUtc { get; }
        public string Category { get; set; }
        public LogLevel Level { get; set; }
        public string Text { get; set; }
        public Exception Exception { get; set; }
        public EventId EventId { get; set; }
        public object State { get; set; }
        public Dictionary<string, object> StateProperties { get; set; }
    }
}