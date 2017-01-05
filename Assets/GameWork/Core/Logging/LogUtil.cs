using System;
using GameWork.Core.Logging.PlatformAdaptors;

namespace GameWork.Core.Logging
{
    public class LogUtil
    {
        private static ILogger _logger;

        public static LogLevel LogLevel { get; set; }
        
        public static void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static void Fatal(Exception exception)
        {
            if (LogLevel.Fatal <= LogLevel)
            {
                _logger.Fatal(exception);
            }
        }

        public static void Fatal(string message)
        {
            if (LogLevel.Fatal <= LogLevel)
            {
                _logger.Fatal(message);
            }
        }

        public static void Error(Exception exception)
        {
            if (LogLevel.Error <= LogLevel)
            {
                _logger.Error(exception);
            }
        }
        
        public static void Error(string message)
        {
            if (LogLevel.Error <= LogLevel)
            {
                _logger.Error(message);
            }
        }

        public static void Warning(string message)
        {
            if (LogLevel.Warning <= LogLevel)
            {
                _logger.Warning(message);
            }
        }

        public static void Debug(string message)
        {
            if (LogLevel.Debug <= LogLevel)
            {
                _logger.Debug(message);
            }
        }
        public static void Info(string message)
        {
            if(LogLevel.Info <= LogLevel)
            {
                _logger.Info(message);
            }
        }
    }
}
