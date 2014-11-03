using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public interface ILogWriter
    {
        void AppendLine(string text);
    }

    public class LogWriter : ILogWriter
    {
        private readonly string _logFilePath;

        public LogWriter(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public void AppendLine(string text)
        {
            if (!string.IsNullOrWhiteSpace(_logFilePath))
                File.AppendAllLines(_logFilePath, new[] { text });
        }
    }
}
