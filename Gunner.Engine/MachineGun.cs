using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                //TODO: calculate total number of requests that this test will perform.
                Console.WriteLine("Endpoints (urls):");
                int x = 0;
                _urls.ToList().ForEach(u=> Console.WriteLine("    {0}.{1}",++x,u));
                Console.WriteLine("---------------------------");
            }
            for (int users =  _options.Start; users < _options.Users; users += _options.Increment)
            {
                TestCocurrentRequests(_options, users,_options.Repeat);
                Thread.Sleep(_options.Pause);
            }
            Console.WriteLine("Total requests:{0}",totalRequests);
            Console.WriteLine("-------- finished ---------");
        }

        static int totalRequests = 0;
        static int batchRequests = 0;
        static int success = 0;
        static int fail = 0;

        public static async Task GetUrlAsync(int requests, HttpClient client, Func<int,bool, string> nextString, string find, bool verbose, int verboseMessagesToShow, bool cachebust)
        {
            int i = requests - 1;
            if (requests == 0) return;
            var url = nextString(i,cachebust);
            Interlocked.Increment(ref batchRequests);
            Interlocked.Increment(ref totalRequests);
            try
            {
                bool verbosed = (verbose && totalRequests < verboseMessagesToShow);
                var result = await client.GetStringAsync(url);
                if (verbosed) Console.WriteLine(result);
                if (result.Contains(find))
                    Interlocked.Increment(ref success);
                else
                {
                    //todo replace with single line view, remove carriage returns.
                    if (verbosed) Console.WriteLine("could not find '{0}' in result: {1}",find,result.First(25));
                    Interlocked.Increment(ref fail);
                }
                    
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref fail);
                if (verbose && totalRequests < VerboseMessagesToShow) Console.WriteLine("EXCEPTION:{0}", ex.Message);
                throw;
            }
        }


        static void TestCocurrentRequests(Options options,int users, int repeat)
        {
            batchRequests = 0;
            success = 0;
            fail = 0;
            var tasks = new List<Task>();
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < users; i++)
            {
                // NB! need to dispose these httpclients!
                var client = new HttpClient();
                var task = GetUrlAsync(repeat, client, GetUrl, options.Find, options.Verbose, VerboseMessagesToShow,options.Cachebuster);
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray(), options.Timeout * 1000);
            sw.Stop();
            float rps = ((float)batchRequests / sw.ElapsedMilliseconds) * 1000;
            float averesponse = (float)sw.ElapsedMilliseconds / (float)batchRequests;
            //todo: move to logging class
            string logline = string.Format(options.Format, DateTime.Now, totalRequests, rps, users, success, fail, averesponse);
            Console.WriteLine(logline);
            //NB! does not keep file open, so that it doesn't lock, and so that it can be monitored in realtime for graphing.
            if(options.LogPath!=null) File.AppendAllLines(options.LogPath, new []{ logline});
        }


        private static string[] _urls;

        // to do, convert to a class, and store an instance of the class into a static
        private static string GetUrl(int i, bool cachebust)
        {
            int len = _urls.Length;
            return _urls[i%len];
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
