using System;
using System.IO;
using Shared.Abstract;

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
            LogString($"[Debug]   {message}");
        }

        public void Debug(string message, Exception exception)
        {
            LogString($"[Debug]   {message}   {exception.Message}");
        }

        public void Error(string message)
        {
            LogString($"[Error]   {message}");
        }

        public void Error(string message, Exception exception)
        {
            LogString($"[Error]   {message}   {exception.Message}");
        }

        public void Info(string message)
        {
            LogString($"[Info]   {message}");
        }

        public void Info(string message, Exception exception)
        {
            LogString($"[Info]   {message}   {exception.Message}");
        }

        public void Warn(string message)
        {
            LogString($"[Warn]   {message}");
        }

        public void Warn(string message, Exception exception)
        {
            LogString($"[Warn]   {message}   {exception.Message}");
        }

        private void LogStart(string microservice)
        {
            var dateTimeFormat = "yyyy-MM-dd_HH_mm_ss";
            var date = DateTime.Now.ToString(dateTimeFormat);
            var logFile = "StockExchangeLog_" + microservice + "_" + date + ".log";
            
            string basePath = @"C:\Logs";
            string logPath = basePath + "\\" + microservice;
            Directory.CreateDirectory(logPath);
            string logFileTest = Path.Combine(logPath, logFile);

            _log = !File.Exists(logFileTest) ? new StreamWriter(logFileTest) : File.AppendText(logFileTest);
        }

        private void LogString(string stringToLog)
        {
            _log.WriteLine(stringToLog);
            _log.Flush();
        }
    }
}
