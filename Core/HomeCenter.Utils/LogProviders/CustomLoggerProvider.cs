﻿using HomeCenter.Utils.ConsoleExtentions;
using Microsoft.Extensions.Logging;
using System;

namespace HomeCenter.Utils.LogProviders
{
    public class ConsoleLogProvider : ILoggerProvider
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CustomConsoleLogger(categoryName);
        }

        public class CustomConsoleLogger : ILogger
        {
            private readonly string _categoryName;
            private readonly object _locki = new object();

            public CustomConsoleLogger(string categoryName)
            {
                _categoryName = categoryName;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }

                lock (_locki)
                {
                    var time = DateTime.Now.ToString("HH:mm:ss.fff");

                    if (logLevel == LogLevel.Error)
                    {
                        ConsoleEx.WriteError($"[E] {time}: ");
                    }
                    else if (logLevel == LogLevel.Warning)
                    {
                        ConsoleEx.WriteWarning($"[W] {time}: ");
                    }
                    else if (logLevel == LogLevel.Information)
                    {
                        ConsoleEx.WriteOK($"[I] {time}: ");
                    }
                    else if (logLevel == LogLevel.Debug)
                    {
                        ConsoleEx.WriteDebug($"[D] {time}: ");
                    }
                    else if (logLevel == LogLevel.Trace)
                    {
                        ConsoleEx.WriteTrace($"[T] {time}: ");
                    }
                    else
                    {
                        Console.Write($"{logLevel}: ");
                    }
                    Console.WriteLine($"{formatter(state, exception)}");
                }
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }
    }
}