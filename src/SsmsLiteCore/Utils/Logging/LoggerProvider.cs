using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace SsmsLite.Core.Utils.Logging
{
    public abstract class LoggerProvider : IDisposable, ILoggerProvider
    {
        ConcurrentDictionary<string, Logger> loggers = new ConcurrentDictionary<string, Logger>();

        protected IDisposable SettingsChangeToken;

        ILogger ILoggerProvider.CreateLogger(string Category)
        {
            return loggers.GetOrAdd(Category,
            (category) => {
                return new Logger(this, category);
            });
        }

        void IDisposable.Dispose()
        {
            if (!this.IsDisposed)
            {
                try
                {
                    Dispose(true);
                }
                catch
                {
                }

                this.IsDisposed = true;
                GC.SuppressFinalize(this);  // instructs GC not bother to call the destructor   
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (SettingsChangeToken != null)
            {
                SettingsChangeToken.Dispose();
                SettingsChangeToken = null;
            }
        }

        public LoggerProvider()
        {
        }

        ~LoggerProvider()
        {
            if (!this.IsDisposed)
            {
                Dispose(false);
            }
        }

        public abstract bool IsEnabled(LogLevel logLevel);

        public abstract void WriteLog(LogEntry Info);

        public bool IsDisposed { get; protected set; }
    }
}
