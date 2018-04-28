using System;

namespace Roadkill.Text
{
    /// <summary>
    /// Manages logging in Roadkill.
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Creates an information log message.
        /// </summary>
        public static void Debug(string message, params object[] args)
        {
            Write(Level.Debug, null, message, args);
        }

        /// <summary>
        /// Creates an information log message.
        /// </summary>
        public static void Information(string message, params object[] args)
        {
            Write(Level.Information, null, message, args);
        }

        /// <summary>
        /// Creates an information log message, also logging the provided exception.
        /// </summary>
        public static void Information(Exception ex, string message, params object[] args)
        {
            Write(Level.Information, ex, message, args);
        }

        /// <summary>
        /// Creates a warning log message.
        /// </summary>
        public static void Warn(string message, params object[] args)
        {
            Write(Level.Warning, null, message, args);
        }

        /// <summary>
        /// Creates a information log message, also logging the provided exception.
        /// </summary>
        public static void Warn(Exception ex, string message, params object[] args)
        {
            Write(Level.Warning, ex, message, args);
        }

        /// <summary>
        /// Creates an error (e.g. application crash) log message.
        /// </summary>
        public static void Error(string message, params object[] args)
        {
            Write(Level.Error, null, message, args);
        }

        /// <summary>
        /// Creates an error (e.g. application crash) log message, also logging the provided exception.
        /// </summary>
        public static void Error(Exception ex, string message, params object[] args)
        {
            Write(Level.Error, ex, message, args);
        }

        /// <summary>
        /// Writes a log message for the <see cref="Level"/>, and if the provided Exception is not null,
        /// appends this exception to the message.
        /// </summary>
        public static void Write(Level errorType, Exception ex, string message, params object[] args)
        {
        }
    }
}