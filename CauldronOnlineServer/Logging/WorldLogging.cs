using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CauldronOnlineServer.Logging
{
    public class WorldLogging
    {
        public delegate void OnLog(string message);

        private const string INFO = "INFO";
        private const string WARNING = "WARNING";
        private const string ERROR = "ERROR";
        private const string DEBUG = "DEBUG";

        private const string LOG_DIRECTORY = "Logs";

        public OnLog OnInfo { get; set; }
        public OnLog OnDebug { get; set; }
        public OnLog OnWarning { get; set; }
        public OnLog OnError { get; set; }

        private ConcurrentQueue<string> _logMessages = new ConcurrentQueue<string>();

        private Thread _loggingThread = null;
        private bool _active = true;

        private string _currentFile = string.Empty;
        private DateTime _fileStamp = DateTime.MinValue;
        private string _logFolderPath = string.Empty;

        public WorldLogging(string path)
        {
            _logFolderPath = $"{path}";
            if (!Directory.Exists(_logFolderPath))
            {
                Directory.CreateDirectory(_logFolderPath);
            }
            RefreshFile();
            _loggingThread = new Thread(SaveLogs);
            _loggingThread.Start();
        }

        public void Info(string message)
        {
            var msg = $"[{INFO}][{DateTime.Now:MM-dd-yyyy HH:mm:ss}] - {message}";
            _logMessages.Enqueue(msg);
            OnInfo?.Invoke(msg);
        }

        public void Warning(string message)
        {
            var msg = $"[{WARNING}][{DateTime.Now:MM-dd-yyyy HH:mm:ss}] - {message}";
            _logMessages.Enqueue(msg);
            OnWarning?.Invoke(msg);
        }

        public void Error(string message)
        {
            var msg = $"[{ERROR}][{DateTime.Now:MM-dd-yyyy HH:mm:ss}] - {message}";
            _logMessages.Enqueue(msg);
            OnError?.Invoke(msg);
        }

        public void Debug(string message)
        {
            var msg = $"[{DEBUG}]-[{DateTime.Now:MM-dd-yyyy HH:mm:ss}] - {message}";
            _logMessages.Enqueue(msg);
            OnDebug?.Invoke(msg);
        }

        private void SaveLogs()
        {
            while (_active)
            {
                var entries = new List<string>();
                while (_logMessages.TryDequeue(out var logEntry))
                {
                    entries.Add(logEntry);
                }
                File.AppendAllLines(_currentFile, entries);
                Thread.Sleep(150);
            }
        }

        private void RefreshFile()
        {
            if (_fileStamp.DayOfYear != DateTime.Now.DayOfYear)
            {
                _fileStamp = DateTime.Now;
                _currentFile = $"{_logFolderPath}{Path.DirectorySeparatorChar}Server-{DateTime.Now:MM-dd-yyyy}.log";
            }
        }

    }
}