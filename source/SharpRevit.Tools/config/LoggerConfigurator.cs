﻿using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRevit.config
{
    /// <summary>
    ///     Application logging configuration
    /// </summary>
    /// <example>
    /// <code lang="csharp">
    /// public class Class(ILogger&lt;Class&gt; logger)
    /// {
    ///     private void Execute()
    ///     {
    ///         logger.LogInformation("Message");
    ///     }
    /// }
    /// </code>
    /// </example>
    public static class LoggerConfigurator
    {
        private const string LogTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";

        public static void AddSerilogConfiguration(this ILoggingBuilder builder)
        {
            var logger = CreateDefaultLogger();
            builder.AddSerilog(logger);

            AppDomain.CurrentDomain.UnhandledException += OnOnUnhandledException;
        }

        private static Logger CreateDefaultLogger()
        {
            //return new LoggerConfiguration()
            //    .WriteTo.Debug(LogEventLevel.Debug, LogTemplate)
            //    .MinimumLevel.Debug()
            //    .CreateLogger();

            return new LoggerConfiguration()
                .WriteTo.File(@"logs/cwoApp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        private static void OnOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var exception = (Exception)args.ExceptionObject;
            var logger = Host.GetService<ILogger<AppDomain>>();
            logger.LogCritical(exception, "Domain unhandled exception");
        }
    }
}
