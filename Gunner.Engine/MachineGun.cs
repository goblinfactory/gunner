using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public class MachineGun
    {
        public bool IsRunning { get; set; }
        private readonly IDownloader _downloader;
        private readonly Options _options;
        private readonly IMetricMonitoring _metricMonitoring;
        private readonly ITrafficMonitor _trafficMonitor;
        private readonly ILogWriter _logWriter;
        private List<BatchRunResult> _batchRunResults;

        public List<BatchRunResult> ReadResults()
        {
            return _batchRunResults;
        }


        public MachineGun(IDownloader downloader, Options options, ILogWriter logWriter, string[] urls, ITrafficMonitor trafficMonitor, IMetricMonitoring metricMonitoring = null)
        {
            _downloader = downloader;
            _options = options;
            _metricMonitoring = metricMonitoring ?? new NullMetricMonitor();
            _trafficMonitor = trafficMonitor;
            _logWriter = logWriter;
            _urls = urls; 
            IsRunning = false;
            _batchRunResults = new List<BatchRunResult>();
        }

        public string[] Urls { get { return _urls; } }

        public void FireOneShotAcrossTheBowAndWakeThatSuckerUp()
        {
            var url = _urls[0] + "?bust=" + Guid.NewGuid();
            using (var client = new WebClient())
            {
                client.DownloadString(url);
            }
        }

        private const int VerboseMessagesToShow = 10;
        public const string Title = "Gunner v 0.1";
        public async Task Run()
        {
            if (_metricMonitoring.SystemBeingMonitoredIsCold) 
                FireOneShotAcrossTheBowAndWakeThatSuckerUp();
            Console.WriteLine(Title);
            Console.WriteLine(Options.DefaultHeader);
            if (_options.Verbose)
            {
                Console.WriteLine("------ test settings ------");
                Console.WriteLine("{0} to {1} step {2}. \n{3} req's/user. {4}ms between batches.",
                _options.Start,
                _options.End,
                _options.Increment,
                _options.Repeat,
                _options.Pause);
                Console.WriteLine("Endpoints (urls):");
                int x = 0;
                _urls.ToList().ForEach(u=> Console.WriteLine("    {0}.{1}",++x,u));
                Console.WriteLine("---------------------------");
            }
            var startMemory = MemoryHelper.GetPeakWorkingSetKb();
            int grandTotal = 0;
            IsRunning = true;
            var sw = new Stopwatch();
            sw.Start();
            int batchNumber = 1;
            for (int batch =  _options.Start; batch <= _options.End; batch += _options.Increment)
            {
                batchNumber++;
                bool skipBatch = (batchNumber <= _options.SkipBatches);
                BatchRunResult batchResult = await TestCocurrentRequests(batchNumber,_downloader, skipBatch, _metricMonitoring, _trafficMonitor, _logWriter, grandTotal, startMemory, _options, batch, _options.Repeat, _options.Gap);
                _batchRunResults.Add(batchResult);
                grandTotal += batchResult.Total;
                Thread.Sleep(_options.Pause);
            }
            sw.Stop();
            IsRunning = false;
            Console.WriteLine("Total requests:{0,7} - Total time:{1,7:0.00}s", grandTotal,(sw.ElapsedMilliseconds/1000M));
            Console.WriteLine("-------- finished ---------");
        }

        //public static DownloadResult Download(string url, WebClient client, string find, bool verbose, int verboseMessagesToShow, bool cachebust, string logPath, bool logErrors)
        //{
        //    var dr = new DownloadResult();
        //    try
        //    {
        //        var result = client.DownloadString(url);
        //        if (result.Contains(find))
        //        {
        //            dr.Success = true;
        //            return dr;
        //        }
        //        dr.Success = false;
        //        return dr;
        //    }
        //    catch (WebException we)
        //    {
        //        if (logErrors) LogError(url, we, logPath);
        //        dr.Success = false;

        //        if (we.Status == WebExceptionStatus.ProtocolError)
        //        {
        //            var response = we.Response as HttpWebResponse;
        //            if (response != null)
        //            {
        //                dr.ErrorCode = (int) response.StatusCode;
        //            }
        //        }
        //        if (logErrors) LogError(url, we, logPath);
        //        return dr;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (logErrors) LogError(url, ex, logPath);
        //        dr.Success = false;
        //        return dr;
        //    }
        //}

        // pause betweenRequests can be used to simulate network latency
        
        static async Task<BatchRunResult> TestCocurrentRequests(int batchNumber, IDownloader downloader, bool skipBatch, IMetricMonitoring metricMonitoring, ITrafficMonitor network, ILogWriter logWriter, int grandTotal, decimal MbStart, Options options, int users, int repeat, int pauseBetweenRequests)
        {
            var batch = new BatchRunResult();
            network.StartMonitoring();

            var tasks = new List<Task<UserRunResult>>();
            var sw = new Stopwatch();
            
            sw.Start();
            batch.TimeStart = DateTime.Now;
            for (int i = 1; i < users+1; i++)
            {
                Task<UserRunResult> task = Task.Run( async () =>
                    {
                        var batchResult = new UserRunResult();

                        using (var client = new WebClient())
                        {
                            // stagger
                            await Task.Delay(new Random().Next(options.StaggerStart));
                            for (int r = 0; r < repeat; r++)
                            {
                                var url = GetUrl(r, options.Cachebuster);
                                var dr = downloader.Download(url, client, options.Find, options.Verbose, VerboseMessagesToShow, options.Cachebuster, options.Logfile, options.LogErrors);
                                batchResult.UpdateTotals(dr);
                                if (pauseBetweenRequests > 0) await Task.Delay(new Random().Next(options.StaggerStart));
                            }                            
                        }
                        return batchResult;
                    });
                tasks.Add(task);
            }
            // Sum the batch results!
            while (tasks.Any())
            {
                //no locking requires since userResult will never "finish" twice!  w00t! love this pattern...
                Task<UserRunResult> userResultTask = await Task.WhenAny(tasks);
                var userResult = await userResultTask;
                tasks.Remove(userResultTask);
                batch.UpdateTotals(userResult);
            }
            sw.Stop();
            batch.TimeStop = DateTime.Now;
            batch.DurationMs = sw.ElapsedMilliseconds;
            batch.Traffic = network.ReadTrafficSinceMonitoringStarted();
            batch.MemoryUsedMb = MemoryHelper.GetPeakWorkingSetKb() - MbStart;
            int total = batch.Total;
            float rps = ((float)total / sw.ElapsedMilliseconds) * 1000;
            float averesponse = (1F/rps) *1000;
            batch.AverageResponseMs = averesponse;
            batch.RequestsPerSecond = rps;
            batch.Metrics = metricMonitoring.ReadMetrics();
            string logline = string.Format(options.Format, DateTime.Now, grandTotal + total, rps, users, batch.Success, batch.Fail, averesponse, batch.Traffic.ReceivedBytes.ToMegabytes(), batch.Traffic.SentBytes.ToMegabytes(), batch.MemoryUsedMb);
            Console.Write(logline);
            Console.WriteLine(" {0}",batch);
            if (logWriter!=null) logWriter.AppendLine(logline + " " + batch);
            return batch;
        }

        public static List<string> _errors = new List<string>(10000);
        public static DateTime LastFlush = DateTime.Now;

        // NB! replace with nlog? (re-invent wheel). this will potentially lose the last 5 seconds of errors, just putting this in here for now as a hacky source of debug info
        // useful to have here as it doesnt require user to configure any log4net or nlog configuration files.
        public static void LogError(string url, Exception ex, string path)
        {
            // NB! this will (may?) cause problems at high concurrency!
            string error = string.Format("ERROR {0} : {1} {2}", url, ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
            _errors.Add(error);
            if (DateTime.Now.Subtract(LastFlush).TotalSeconds>5)
            {
                LastFlush = DateTime.Now;
                // NB! should check if file is locked! Will replace with nlog so don't want to gold plate this.
                File.AppendAllLines(path, _errors.ToArray());
                _errors.Clear();
            }
        }


        private static string[] _urls;

        private static string GetUrl(int i, bool cachebust)
        {
            int len = _urls.Length;
            var url = _urls[new Random().Next(len)];
            return cachebust ? UrlReader.Bust(url) : url;
        }
    }
}
