using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public class MachineGun
    {
        private readonly Options _options;

        public MachineGun(Options options)
        {
            _options = options;
            _urls = new UrlReader(options).ReadUrls(Environment.CurrentDirectory);
        }

        private const int VerboseMessagesToShow = 10;
        public const string Title = "Gunner v 0.1";

        public void Run()
        {
            Console.WriteLine(Title);
            Console.WriteLine(Options.DefaultHeader);
            if (_options.Verbose)
            {
                Console.WriteLine("------ test settings ------");
                Console.WriteLine("{0} to {1} step {2}. \n{3} req's/user. {4}ms between batches.",
                _options.Start,
                _options.Users,
                _options.Increment,
                _options.Repeat,
                _options.Pause);
                Console.WriteLine("Endpoints (urls):");
                int x = 0;
                _urls.ToList().ForEach(u=> Console.WriteLine("    {0}.{1}",++x,u));
                Console.WriteLine("---------------------------");
            }
            for (int batch =  _options.Start; batch < _options.Users; batch += _options.Increment)
            {
                TestCocurrentRequests(_options, batch,_options.Repeat,_options.Gap);
                Thread.Sleep(_options.Pause);
            }
            //Console.WriteLine("Total requests:{0}",totalRequests);
            Console.WriteLine("-------- finished ---------");
        }

        public static async Task<DownloadResult> DownloadAsync(string url, WebClient client, string find, bool verbose, int verboseMessagesToShow, bool cachebust, string logPath, bool logErrors)
        {
            var dr = new DownloadResult();
            try
            {
                var result = await client.DownloadStringTaskAsync(url);
                if (result.Contains(find))
                {
                    dr.Success = true;
                    return dr;
                }
                dr.Success = false;
                return dr;
            }
            catch (WebException we)
            {
                if (logErrors) LogError(url, we, logPath);
                dr.Success = false;

                if (we.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = we.Response as HttpWebResponse;
                    if (response != null)
                    {
                        dr.ErrorCode = (int)response.StatusCode;
                    }
                }
                if (logErrors) LogError(url, we, logPath);
                return dr;
            }
            catch (Exception ex)
            {
                if (logErrors) LogError(url, ex, logPath);
                dr.Success = false;
                return dr;
            }
        }


        public static DownloadResult DownloadOld(string url, WebClient client, string find, bool verbose, int verboseMessagesToShow, bool cachebust, string logPath, bool logErrors)
        {
            var dr = new DownloadResult();
            try
            {
                var result = client.DownloadString(url);
                if (result.Contains(find))
                {
                    dr.Success = true;
                    return dr;
                }
                dr.Success = false;
                return dr;
            }
            catch (WebException we)
            {
                if (logErrors) LogError(url, we, logPath);
                dr.Success = false;

                if (we.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = we.Response as HttpWebResponse;
                    if (response != null)
                    {
                        dr.ErrorCode = (int)response.StatusCode;
                    }
                }
                if (logErrors) LogError(url, we, logPath);
                return dr;
            }
            catch (Exception ex)
            {
                if (logErrors) LogError(url, ex, logPath);
                dr.Success = false;
                return dr;
            }
        }

        // pause betweenRequests can be used to simulate network latency
        static async void TestCocurrentRequests(Options options,int users, int repeat, int pauseBetweenRequests)
        {
            var tasks = new List<Task<UserRunResult>>();
            var sw = new Stopwatch();
            
            sw.Start();
            for (int i = 0; i < users; i++)
            {
                Task<UserRunResult> task = Task.Run( async () =>
                    {
                        var batchResult = new UserRunResult();
                        var client = new WebClient();
                        
                        for (int r = 0; r < repeat; r++)
                        {
                            var url = GetUrl(r, options.Cachebuster);
                            var dr = await DownloadAsync(url, client, options.Find, options.Verbose, VerboseMessagesToShow, options.Cachebuster, options.Logfile, options.LogErrors);
                            batchResult.UpdateTotals(dr);
                            if (pauseBetweenRequests>0) await Task.Delay(pauseBetweenRequests);
                        }
                        return batchResult;
                    });
                tasks.Add(task);
            }
            // Sum the batch results!

            //NB! do while no more users waiting to finish to WhenAny?? tally the batch totals!  (BatchTotal to derive from UserTotal )

            //Task.WaitAll(tasks.ToArray(), options.Timeout * 1000);
            var batch = new BatchRunResult();
            while (tasks.Count() > 0)
            {
                //no locking requires since userResult will never "finish" twice!  w00t! love this pattern...
                //Q: does this block my cancellation in this loop?
                Task<UserRunResult> userResultTask = await Task.WhenAny(tasks);
                var userResult = await userResultTask;
                tasks.Remove(userResultTask);
                batch.UpdateTotals(userResult);
            }


            sw.Stop();

            // Move to a status class
            int total = batch.Total;
            float rps = ((float)total / sw.ElapsedMilliseconds) * 1000;
            float averesponse = 1F/rps; 
            //todo: move to logging class
            string logline = string.Format(options.Format, DateTime.Now, total, rps, users, batch.Success, batch.Fail, averesponse);
            Console.Write(logline);
            Console.WriteLine(" {0}",batch);
            //NB! does not keep file open, so that it doesn't lock, and so that it can be monitored in realtime for graphing.
            if (!string.IsNullOrWhiteSpace(options.Logfile)) File.AppendAllLines(options.Logfile, new[] { logline + " " + batch.ToString() });
        }

        public static List<string> _errors = new List<string>(10000);
        public static DateTime LastFlush = DateTime.Now;

        // NB! replace with nlog! this will potentially lose the last 5 seconds of errors, just putting this in here for now as a hacky source of debug info

        
        public static void LogError(string url, Exception ex, string path)
        {
            // NB! this will cause problems at high concurrency!
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

        // to do, convert to a class, and store an instance of the class into a static
        private static string GetUrl(int i, bool cachebust)
        {
            int len = _urls.Length;
            return _urls[i%len];
        }
    }

}
