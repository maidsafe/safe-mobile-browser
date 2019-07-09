using System;
using System.IO;

namespace SafeMobileBrowser.Services
{
    public class ConsoleLoggingService : ILoggingService
    {
        /// <inheritdoc />
        public virtual void Log(LogType logType, string message)
        {
            WriteToConsole(logType, message);
        }

        /// <inheritdoc />
        public virtual void Log(LogType logType, string message, Exception exception)
        {
            WriteToConsole(logType, message + "\n" + GetExceptionDetails(exception));
        }

        protected virtual void WriteToConsole(LogType type, string message)
        {
            Console.WriteLine("[{0}] {1}", type, message);
        }

        protected virtual string GetExceptionDetails(Exception exception)
        {
            var writer = new StringWriter();

            writer.Write("Exception: ");
            writer.WriteLine(exception.GetType());
            writer.Write("Message: ");
            writer.WriteLine(exception.Message);
            if (exception.InnerException != null)
            {
                writer.Write("InnerException: ");
                writer.WriteLine(exception.InnerException);
            }
            writer.Write("StackTrace: ");
            writer.WriteLine(exception.StackTrace);

            return writer.ToString();
        }
    }
}
