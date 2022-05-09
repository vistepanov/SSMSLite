using Microsoft.Extensions.Logging;

namespace SsmsLite.Core.Utils.Logging
{
    public class FileLoggerOptions
    {
        public string Folder { get; set; }

        public int RetainPolicyFileCount { get; set; } = 10;

        public LogLevel LogLevel { get; set; } = LogLevel.Information;
    }
}
