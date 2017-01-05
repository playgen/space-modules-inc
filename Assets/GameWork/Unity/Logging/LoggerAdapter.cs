using System;
using UnityEngine;
using ILogger = GameWork.Core.Logging.PlatformAdaptors.ILogger;

namespace GameWork.Unity.Logging
{
    public class LoggerAdapter : ILogger
    {
        private readonly UnityEngine.ILogger _logger = UnityEngine.Debug.logger;

        public void Debug(string message)
        {
            _logger.Log(LogType.Log, message);
        }

        public void Error(Exception exception)
        {
            _logger.Log(LogType.Exception, exception);
        }

        public void Error(string message)
        {
            _logger.Log(LogType.Error, message);
        }

        public void Fatal(Exception exception)
        {
            _logger.Log(LogType.Exception, exception);
        }

        public void Fatal(string message)
        {
            _logger.Log(LogType.Error, message);
        }

        public void Info(string message)
        {
            _logger.Log(LogType.Log, message);
        }

        public void Warning(string message)
        {
            _logger.Log(LogType.Warning, message);
        }
    }
}