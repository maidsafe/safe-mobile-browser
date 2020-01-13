// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

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
