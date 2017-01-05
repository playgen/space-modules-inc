using System;

namespace GameWork.Core.Logging.PlatformAdaptors
{
    public interface ILogger
    {
        /// <summary>
        /// Information tracking the usage of the system.
        /// </summary>
        /// <param name="message"></param>
        void Info(string message);

        /// <summary>
        /// Information to help with debugging
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
        /// Issues that break gameplay/user experience.
        /// </summary>
        /// <param name="exception"></param>
        void Error(Exception exception);

        /// <summary>
        /// Issues that would/should crash the game.
        /// </summary>
        /// <param name="message"></param>
        void Fatal(string message);

        /// <summary>
        /// Issues that would/should crash the game.
        /// </summary>
        /// <param name="exception"></param>
        void Fatal(Exception exception);
    }
}