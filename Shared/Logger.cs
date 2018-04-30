using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shared
{
    public class Logger : ILogger
    {
        private static StreamWriter _log;

        public Logger(string microserviceName)
        {
            LogStart(microserviceName);
        }

        public void Debug(string message)
        {
            LogString(message, _log);
        }

        public void Debug(string message, Exception exception)
        {
            LogString(message + " " + exception.Message , _log);
        }

        public void Error(string message)
        {
            LogString(message, _log);
        }

        public void Error(string message, Exception exception)
        {
            LogString(message + " " + exception.Message + " " + exception.StackTrace, _log);
        }

        public void Info(string message)
        {
            LogString(message, _log);
        }

        public void Info(string message, Exception exception)
        {
            LogString(message + " " + exception.Message, _log);
        }

        public void Warn(string message)
        {
            LogString(message, _log);
        }

        public void Warn(string message, Exception exception)
        {
            LogString(message + " " + exception.Message, _log);
        }

        private void LogStart(string microservice)
        {
            var dateTimeFormat = "MM-dd-yyy_HH_mm_ss";
            var date = DateTime.Now.ToString(dateTimeFormat);
            var logFile = "StockExchangeLog " + microservice + " " + date + ".log";

            string logPath = @"c:\Logs";
            string logFileTest = Path.Combine(logPath, logFile);

            _log = !File.Exists(logFileTest) ? new StreamWriter(logFileTest) : File.AppendText(logFileTest);
        }

        private void LogString(string stringToLog, StreamWriter log)
        {
            log.WriteLine(stringToLog);
            log.Flush();
        }
    }
}
