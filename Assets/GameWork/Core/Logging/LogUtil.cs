using System;
using System.Diagnostics;
using GameWork.Core.Logging.Interfaces;

namespace GameWork.Core.Logging
{
    public class LogUtil
    {
        private static ILogger _logger;

        public LogUtil(ILogger logger)
        {
            _logger = logger;
        }

        [Conditional("LOGGING_ENABLED_DEBUG"), Conditional("LOGGING_ENABLED_ALL")]
        public static void Debug(string message)
        {
            _logger.Debug(message);
        }

        [Conditional("LOGGING_ENABLED_WARNING"), Conditional("LOGGING_ENABLED_ALL")]
        public static void Warning(string message)
        {
            _logger.Warning(message);
        }

        [Conditional("LOGGING_ENABLED_ERROR"), Conditional("LOGGING_ENABLED_ALL")]
        public static void Error(string message)
        {
            _logger.Error(message);
        }

        [Conditional("LOGGING_ENABLED_EXCEPTION"), Conditional("LOGGING_ENABLED_ALL")]
        public static void Exception(Exception exception)
        {
            _logger.Exception(exception);
        }
    }
}
