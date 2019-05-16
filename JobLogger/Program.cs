using JobLogger.Enum;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JobLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            // You can add JobLogger as a service
            new ServiceCollection()
            .AddJobLogger(LogTarget.All, LogType.All)
            .BuildServiceProvider();

            // How to use the logguer
            JobLogger.LogMessage(LogLevel.Message, "This is a test info message");
            JobLogger.LogMessage(LogLevel.Error, "This is a test error message");
            JobLogger.LogMessage(LogLevel.Warning, "This is a test warning message");

            Console.WriteLine("\nThe execution has ended.");
            Console.ReadKey();
        }
    }
}
