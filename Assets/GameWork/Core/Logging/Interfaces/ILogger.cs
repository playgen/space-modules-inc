using System;

namespace GameWork.Core.Logging.Interfaces
{
    public interface ILogger
    {
        /// <summary>
        /// Information to help with debugging
        /// Be careful not to log too much information.
        /// </summary>
        /// <param name="message"></param>
        void Debug(string message);

        /// <summary>
        /// Issues that should be addressed but do not break gameplay/user experience.
        /// </summary>
        /// <param name="message"></param>
        void Warning(string message);

        /// <summary>
        /// Issues that break gameplay/user experience.
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);

        /// <summary>
        /// Issues that would/should crash the game.
        /// </summary>
        /// <param name="exception"></param>
        void Exception(Exception exception);
    }
}