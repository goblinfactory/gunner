using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public interface IReporterFormat
    {
        string Format { get;  }
        string Header { get; }
    }

    public interface IReporter
    {
        void WriteFormattedBatchResultHeading();
        void WriteFormattedBatchLineResult(BatchRunResult batch);
    }

    public class Reporter : IReporter
    {
        private readonly IReporterFormat _format;
        private readonly ILogWriter _logwriter;

        public Reporter(IReporterFormat format, ILogWriter logwriter)
        {
            _format = format;
            _logwriter = logwriter;
        }

        public void WriteFormattedBatchResultHeading()
        {
            Console.WriteLine(_format.Header);
        }

        public void WriteFormattedBatchLineResult(BatchRunResult batch)
        {
            string logline = String.Format(_format.Format, 
                batch.TimeStop, 
                batch.GrandTotalRequests, 
                batch.RequestsPerSecond, 
                batch.Users, 
                batch.Success, 
                batch.Fail, 
                batch.AverageResponseMs, 
                batch.Traffic.ReceivedBytes.ToMegabytes(), 
                batch.Traffic.SentBytes.ToMegabytes(), 
                batch.MemoryUsedMb
            );
            Console.Write(logline);
            Console.WriteLine(" {0}", batch);
            if (_logwriter != null) _logwriter.WriteLine(logline + " " + batch);
        }
    }
}
