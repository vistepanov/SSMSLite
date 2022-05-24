using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace SsmsLite.Core.Utils.Logging
{
    public abstract class LoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, Logger> _loggers = new ConcurrentDictionary<string, Logger>();

        private IDisposable _settingsChangeToken;

        ILogger ILoggerProvider.CreateLogger(string cat)
        {
            return _loggers.GetOrAdd(cat, category => new Logger(this, category));
        }

        void IDisposable.Dispose()
        {
            if (IsDisposed) return;
            try
            {
                Dispose(true);
            }
            catch
            {
                // ignored
            }

            IsDisposed = true;
            GC.SuppressFinalize(this);  // instructs GC not bother to call the destructor   
        }

        protected void Dispose(bool disposing)
        {
            if (_settingsChangeToken == null) return;
            _settingsChangeToken.Dispose();
            _settingsChangeToken = null;
        }


        public abstract bool IsEnabled(LogLevel logLevel);

        public abstract void WriteLog(LogEntry info);

        public bool IsDisposed { get; protected set; }
    }
}
