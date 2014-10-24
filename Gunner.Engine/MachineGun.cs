using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            Urls = _options.Urls; // argh, messy, static global, todo fix this up, some D/I.
        }
        private const int VerboseMessagesToShow = 10;
        
        //const string format = "{0,-7},{1,-11:0.00},{2,-7},{3,-7},{4,-7}, {5,-7:0.0000}ms";
        //const int numUsers = 20000;
        //const int pauseBetweenTests = 8000;
        //const int repeatTests = 100;
        //const int start = 500;
        //const int increment = 100;
        //static bool cachebusterOn = true;

        // to monitor with perfmon add : W3SVC_W3WP 
        // -> requests / Sec 
        // -> total Http Requests served 
        // -> average response?

        //TODO: introduce console writer for testing

        public const string Title = "Gunner v 0.1";

        public void Run()
        {
            Console.WriteLine(Title);
            for (int i =  _options.Start; i < _options.Users; i += _options.Increment)
            {
                for (int r = 0; r < _options.Repeat; r++) TestCocurrentRequests(_options, i);
                Thread.Sleep(_options.Pause);
            }
            Console.WriteLine("--- finished ---");
        }

        static int requests = 0;
        static int success = 0;
        static int fail = 0;


        static void TestCocurrentRequests(Options options, int batchSize)
        {
            requests = 0;
            success = 0;
            fail = 0;
            var tasks = new List<Task>();
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < batchSize; i++)
            {

                // do a parallel.foreach inside each task ;-D
                var task = new Task(() =>
                {
                    try
                    {
                        string cachebuster = options.Cachebuster ? "?buster=" + Guid.NewGuid().ToString() : "";
                        var url = GetUrl(i) + cachebuster;
                        var client = new System.Net.WebClient();
                        Interlocked.Increment(ref requests);
                        var result = client.DownloadString(url);
                        if (options.Verbose && requests < VerboseMessagesToShow)

                        if (result.Contains(options.Find))
                        {
                            Interlocked.Increment(ref success);
                        }
                        else
                            Interlocked.Increment(ref fail);
                    }
                    catch (Exception ex)
                    {
                        if (options.Verbose && requests < VerboseMessagesToShow)
                        {
                            Console.WriteLine("EXCEPTION:{0}",ex.Message);
                        }
                        Interlocked.Increment(ref fail);
                    }
                });
                tasks.Add(task);
                task.Start();
            }
            // this will block
            Task.WaitAll(tasks.ToArray(), options.Timeout * 1000);
            sw.Stop();
            float rps = ((float)requests / sw.ElapsedMilliseconds) * 1000;
            float averesponse = (float)sw.ElapsedMilliseconds / (float)requests;
            // this is not actually waiting for all threads??
            //todo: move to logging class
            string logline = string.Format(options.Format, requests, rps, batchSize, success, fail, averesponse);
            Console.WriteLine(logline);
            //NB! does not keep file open, so that it doesn't lock, and so that it can be monitored in realtime for graphing.
            if(options.LogPath!=null) File.AppendAllLines(options.LogPath, new []{ logline});
        }

        private static string[] Urls;

        // to do, convert to a class, and store an instance of the class into a static
        private static string GetUrl(int i)
        {
            int len = Urls.Length;
            return Urls[i%len];
        }

        // where's that library that makes sending paramters to console apps easy?
        // kinda like a command line arguments model binder?
        // ---------
        // todo:
        // setup perfmon again, check the rps!
        // - write to logfile
        // - ability to co-ordinate requests from another machine (listen on port, self hosted)
        // - add in console command line arguments support, move to console application
        // - consider edge cases?
        // - format string in app.config
        // - xcopy deploy?
        // - graceful fail (wrap whole thing in try catch?) 
        // -
        // load onto one of the test servers
        // hit dev (find dev end point)
        // watch dropwizard endpoint
        // ability to inject a "story" script from linqpad, i.e. use linqpad to edit the lambda that gets passed to the 
        // test "runner". 
        // 
    }

}
