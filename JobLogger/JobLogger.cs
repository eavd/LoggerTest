using JobLogger.Enum;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace JobLogger
{
    /// <summary>
    /// Custom Logger Framework
    /// </summary>
    public static class JobLogger
    {
        private static bool _initialized;
        private static bool _logToFile;
        private static bool _logToConsole;
        private static bool _logToDatabase;
        private static bool _logMessage;
        private static bool _logWarning;
        private static bool _logError;

        /// <summary>
        /// Add service of the Loggin Framework.
        /// The registration will be made in all places and of all types.
        /// </summary>
        public static IServiceCollection AddJobLogger(this IServiceCollection services)
        {
            ConfigureLogTarget(LogTarget.All);
            ConfigureLogType(LogType.All);
            _initialized = true;
            return services;
        }

        /// <summary>
        /// Add service of the Loggin Framework.
        /// The registration will be made in the specific place or places and of all types.
        /// </summary>
        /// <param name="logTarget">Specifies the places where the log is made</param>
        public static IServiceCollection AddJobLogger(this IServiceCollection services, LogTarget logTarget)
        {
            ConfigureLogTarget(logTarget);
            ConfigureLogType(LogType.All);
            _initialized = true;
            return services;
        }

        /// <summary>
        /// Add service of the Loggin Framework
        /// The registration will be made in all places with the type or types specified.
        /// </summary>
        /// <param name="logType">Specifies the type or types of what is going to be logged</param>
        public static IServiceCollection AddJobLogger(this IServiceCollection services, LogType logType)
        {
            ConfigureLogTarget(LogTarget.All);
            ConfigureLogType(logType);
            _initialized = true;
            return services;
        }

        /// <summary>
        /// Add service of the Loggin Framework
        /// The registration will be made in the specified place or places with the specified type or types.
        /// </summary>
        /// <param name="logTarget">Specifies the places where the log is made</param>
        /// <param name="logType">Specifies the type or types of what is going to be logged</param>
        public static IServiceCollection AddJobLogger(this IServiceCollection services, LogTarget logTarget, LogType logType)
        {
            ConfigureLogTarget(logTarget);
            ConfigureLogType(logType);
            _initialized = true;
            return services;
        }

        /// <summary>
        /// Method that is responsible for do the log.
        /// </summary>
        /// <param name="logLevel">The level of the severity of the Log</param>
        /// <param name="Message">Message that will be registered</param>
        public static void LogMessage(LogLevel logLevel, string Message)
        {
            if (!_initialized)
            {
                throw new NotImplementedException("JobLogger must be Implemented");
            }

            if (string.IsNullOrEmpty(Message))
            {
                throw new ArgumentNullException("Message must be specified");
            }

            if (!((_logMessage && (int)logLevel == 1) ||
                (_logError && (int)logLevel == 2) ||
                (_logWarning && (int)logLevel == 3)))
            {
                throw new InvalidOperationException("The LogType does not match with the LogLevel");
            }

            var DateTimeUtc = DateTime.UtcNow;

            if (_logToDatabase)
            {
                using (var connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO Log VALUES('" + Message + "', " + (int)logLevel + ", '" + DateTimeUtc.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + "')";

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }

            if (_logToConsole)
            {
                ConsoleColor consoleoregroundColor;
                switch (logLevel)
                {
                    case LogLevel.Error:
                        consoleoregroundColor = ConsoleColor.Red;
                        break;
                    case LogLevel.Warning:
                        consoleoregroundColor = ConsoleColor.Yellow;
                        break;
                    default:
                        consoleoregroundColor = ConsoleColor.White;
                        break;
                }

                Console.ForegroundColor = consoleoregroundColor;
                Console.WriteLine(DateTimeUtc.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + " " + Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            if (_logToFile)
            {
                File.WriteAllText(string.Format("{0}LogFile_{1}.txt", ConfigurationManager.AppSettings["LogFileDirectory"], DateTimeUtc.ToString("MM.dd.yyyy_HH.mm.ss.fff", CultureInfo.InvariantCulture)), string.Format("Error Serverity: {0}: {1}", (int)logLevel, Message));
            }
            
        }

        #region Helpers
        private static void ConfigureLogType(LogType logType)
        {
            switch (logType)
            {
                case LogType.Message:
                    _logMessage = true;
                    break;
                case LogType.Error:
                    _logError = true;
                    break;
                case LogType.Waning:
                    _logWarning = true;
                    break;
                case LogType.MessageAndError:
                    _logMessage = true;
                    _logError = true;
                    break;
                case LogType.MessageAndWarning:
                    _logMessage = true;
                    _logWarning = true;
                    break;
                case LogType.ErrorAndWarning:
                    _logError = true;
                    _logWarning = true;
                    break;
                case LogType.All:
                    _logError = true;
                    _logWarning = true;
                    _logMessage = true;
                    break;
            }
        }

        private static void ConfigureLogTarget(LogTarget logTarget)
        {
            switch (logTarget)
            {
                case LogTarget.Console:
                    _logToConsole = true;
                    break;
                case LogTarget.Database:
                    _logToDatabase = true;
                    break;
                case LogTarget.File:
                    _logToFile = true;
                    break;
                case LogTarget.ConsoleAndDatabase:
                    _logToConsole = true;
                    _logToDatabase = true;
                    break;
                case LogTarget.ConsoleAndFile:
                    _logToConsole = true;
                    _logToFile = true;
                    break;
                case LogTarget.DatabaseAndFile:
                    _logToDatabase = true;
                    _logToFile = true;
                    break;
                case LogTarget.All:
                    _logToDatabase = true;
                    _logToFile = true;
                    _logToConsole = true;
                    break;
            }
        }
        #endregion
    }
}
