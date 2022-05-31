using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SsmsLite.Core.Utils.Logging
{
    [ProviderAlias("FileCSV")]
    public class FileCsvLoggerProvider : LoggerProvider
    {
        private const int TimeLength = 24;
        private const int LevelLength = 14;
        private const int CategoryLength = 60;
        private readonly ConcurrentQueue<CsvLogEntry> _infoQueue = new ConcurrentQueue<CsvLogEntry>();
        private string _currentFilePath;

        private void ApplyRetainPolicy()
        {
            try
            {
                var fileList = new DirectoryInfo(Settings.Folder)
                    .GetFiles("*.log", SearchOption.TopDirectoryOnly)
                    .OrderBy(fi => fi.CreationTime)
                    .ToList();

                while (fileList.Count >= Settings.RetainPolicyFileCount)
                {
                    var fi = fileList.First();
                    fi.Delete();
                    fileList.Remove(fi);
                }
            }
            catch
            {
                //Do nothing
            }
        }

        private static string Pad(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "".PadRight(maxLength);

            //if (Text.Length > MaxLength)
            //    return Text.Substring(0, MaxLength);
            return text.PadRight(maxLength);
        }

        private void BeginFile()
        {
            Directory.CreateDirectory(Settings.Folder);
            _currentFilePath =
                Path.Combine(Settings.Folder, "log_" + DateTime.Now.ToString("yyyy-MM-dd@HH_mm") + ".csv");

            //using (var writer = new StreamWriter(this._currentFilePath))
            //using (var csv = new CsvWriter(writer))
            //{
            //    csv.WriteHeader<CsvLogEntry>();
            //    csv.NextRecord();
            //}
            ApplyRetainPolicy();
        }

        private static CsvLogEntry ConvertToCsvLogEntry(LogEntry info)
        {
            var csvEntry = new CsvLogEntry
            {
                LocalDate = Pad(info.TimeStampUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.ff"), TimeLength),
                Level = Pad(info.Level.ToString(), LevelLength),
                Category = Pad(info.Category, CategoryLength),
                Text = info.Text
            };

            //if (Info.StateProperties != null && Info.StateProperties.Count > 0)
            //{
            //    csvEntry.Properties = JsonConvert.SerializeObject(Info.StateProperties);
            //}
            return csvEntry;
        }

        public FileCsvLoggerProvider(FileLoggerOptions settings)
        {
            Settings = settings;
            BeginFile();
        }

        public override bool IsEnabled(LogLevel logLevel)
        {
            var result = logLevel != LogLevel.None
                         && Settings.LogLevel != LogLevel.None
                         && Convert.ToInt32(logLevel) >= Convert.ToInt32(Settings.LogLevel);
            return result;
        }

        public override void WriteLog(LogEntry info)
        {
            _infoQueue.Enqueue(ConvertToCsvLogEntry(info));
            Task.Delay(1000).ContinueWith((t) => SavePendingLogs());
        }

        private void SavePendingLogs()
        {
            // prevent concurrent access to the file
            lock (_infoQueue)
            {
                var logEntries = new List<CsvLogEntry>();

                while (_infoQueue.TryDequeue(out var logEntry))
                {
                    logEntries.Add(logEntry);
                }

                if (logEntries.Count > 0)
                {
                    WriteLines(logEntries);
                }
            }
        }

        private void WriteLines(IEnumerable<CsvLogEntry> csvLogLines)
        {
            using (var writer = new StreamWriter(_currentFilePath, append: true))
            {
                foreach (var line in csvLogLines)
                {
                    writer.WriteLine($"{line.LocalDate} {line.Level} {line.Category} {line.Text}");
                }
            }
        }

        internal FileLoggerOptions Settings { get; }
    }
}