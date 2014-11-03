using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gunner.Engine;
using console = System.Console;

namespace Gunner.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Run(args);
            }
            catch (Exception ex)
            {
                console.WriteLine("Error occurred, stopping.");
                
                if (ex.InnerException!=null)
                    console.WriteLine("Error was:{0}",ex.InnerException.Message);
                else
                    console.WriteLine("Error was:{0}", ex.Message);
            }

        }

        private static void Run(string[] args)
        {
            var options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options)) return;
            if (options.Csv.HasText() && options.UrlList.HasText()) throw new ApplicationException("You cannot provide a value for csv as well as urls, only 1 option can be selected.");
            if (options.End > 5000) throw new ApplicationException("Maximum value atmo for users is 5000. This is due to socket connection limit. Later will detect if user can support more, if not show message how to tell OS you want more.");
            if (!string.IsNullOrWhiteSpace(options.Logfile))
            {
                var path = Path.Combine(Environment.CurrentDirectory, options.Logfile);
                File.Create(path).Close();
                options.Logfile = path;
            }
            // for now I'm only monitoring local IIS 
            // later will update to be able to monitor remote window servers, and as a bonus remote apache, jetty dropwizard etc.
            var metricMonitoring = new MetricMonitoring(PerformanceMetric.TotalHttpRequestsServed, PerformanceMetric.RequestsPerSecond);
            ILogWriter logwriter = new LogWriter(options.Logfile);
            var networkMonitor = new NetworkTrafficMonitor();
            var mg = new MachineGun(options, metricMonitoring,networkMonitor, logwriter);

            Task.WaitAll(new[] {mg.Run()});

        }

    }
}
