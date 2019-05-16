using JobLogger.Enum;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace JobLogger.Test
{
    [TestClass]
    public class ExpectedExceptions
    {
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException), "JobLogger must be Implemented")]
        public void LogMessage_WhenCalled_ThrowNotImplementedException()
        {
            // Arrange => Act
            JobLogger.LogMessage(LogLevel.Message, "This is a info message");

            // Assert - ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "The LogType does not match with the LogLevel")]
        public void LogMessage_WhenCalled_ThrowInvalidOperationException()
        {
            // Arrange
            new ServiceCollection()
            .AddJobLogger(LogTarget.Console, LogType.Message)
            .BuildServiceProvider();

            // Act
            JobLogger.LogMessage(LogLevel.Error, "This is a info message");

            // Assert - ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Message must be specified")]
        public void LogMessage_WhenCalled_ThrowArgumentNullException()
        {
            // Arrange
            new ServiceCollection()
            .AddJobLogger(LogTarget.Console)
            .BuildServiceProvider();

            // Act
            JobLogger.LogMessage(LogLevel.Message, "");

            // Assert - ExpectedException
        }
    }
}
