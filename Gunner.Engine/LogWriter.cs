using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public interface ILogWriter
    {
        void WriteLine(string line);
        void Write(string text);
    }

    public static class LogWriterExtensions
    {

        public static void WriteLine(this ILogWriter writer, string format, params object[] args)
        {
            var text = string.Format(format, args);
            writer.WriteLine(text);
        }


        public static void WriteLine(this ILogWriter[] writers, string format, params object[] args)
        {
            var text = string.Format(format, args);
            foreach (var writer in writers) writer.WriteLine(text);
        }

        public static void Write(this ILogWriter writer, string format, params object[] args)
        {
            var text = string.Format(format, args);
            writer.Write(text);
        }

        public static void Write(this ILogWriter[] writers,string format, params object[] args)
        {
            var text = string.Format(format, args);
            foreach (var writer in writers) writer.Write(text);
        }
    }


    public class LogWriter : ILogWriter
    {
        private readonly string _logFilePath;

        public LogWriter(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public void WriteLine(string line)
        {
            if (!string.IsNullOrWhiteSpace(_logFilePath))
                File.AppendAllLines(_logFilePath, new[] { line });
        }

        public void Write(string text)
        {
            if (!string.IsNullOrWhiteSpace(_logFilePath))
                File.AppendAllText(_logFilePath, text);
        }
    }
}
