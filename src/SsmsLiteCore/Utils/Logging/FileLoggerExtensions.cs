using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SsmsLite.Core.Utils.Logging
{
    public static class FileLoggerExtensions
    {
        public static void AddFileLogger(this ILoggingBuilder builder, Func<FileLoggerOptions> configure)
        {
            builder.Services.AddSingleton<ILoggerProvider, FileCsvLoggerProvider>
            (
                services => new FileCsvLoggerProvider(configure())
            );

            // builder.AddFilter<FileLoggerProvider>(null, LogLevel.Trace);
        }

        public static void AddSLogger(this ILoggingBuilder builder, Func<FileLoggerOptions> configure)
        {
            //builder.Services.AddSingleton<ILoggerProvider, SerilogLoggerProvider>(services =>
            //    new SerilogLoggerProvider(logger, true));

            //builder.Services.AddSingleton<ILoggerProvider, FileCsvLoggerProvider>
            //(
            //    services => new FileCsvLoggerProvider(configure())
            //);

            // builder.AddFilter<FileLoggerProvider>(null, LogLevel.Trace);
        }
    }
}