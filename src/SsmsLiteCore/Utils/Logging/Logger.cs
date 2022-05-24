using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace SsmsLite.Core.Utils.Logging
{
    internal class Logger : ILogger
    {
        private readonly LoggerProvider _provider;
        private readonly string _category;

        public Logger(LoggerProvider provider, string category)
        {
            _provider = provider;
            _category = category;
        }

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return _provider.IsEnabled(logLevel);
        }

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId,
            TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!(this as ILogger).IsEnabled(logLevel))
                return;

            var info = new LogEntry
            {
                Category = _category,
                Level = logLevel,
                Text = formatter(state, exception) + (exception != null ? "\n" + exception : ""),
                Exception = exception,
                EventId = eventId,
                State = state
            };


            if (state is IEnumerable<KeyValuePair<string, object>> properties)
            {
                info.StateProperties = new Dictionary<string, object>();

                foreach (KeyValuePair<string, object> item in properties)
                {
                    if (item.Key == "{OriginalFormat}")
                        continue;

                    info.StateProperties[item.Key] = item.Value;
                }
            }

            _provider.WriteLog(info);
        }
    }

    public class NoopDisposable : IDisposable
    {
        public static readonly NoopDisposable Instance = new NoopDisposable();

        public void Dispose()
        {
        }
    }
}