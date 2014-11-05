//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Gunner.Engine
//{
//    /// <summary>
//    /// Warms up a webserver so that performance monitoring starts recording.
//    /// </summary>
//    public class Heater
//    {
//        private readonly Func<int, bool,string> _geturl;
//        private readonly bool _verbose;

//        public Heater(Func<int,bool,string> geturl, bool verbose)
//        {
//            _geturl = geturl;
//            _verbose = verbose;
//        }

//        public static void FireOneShotAcrossTheBowAndWakeThatSuckerUp(string url)
//        {
//            using (var client = new WebClient())
//            {
//                client.DownloadString(url);
//            }
//        }

//        public void WarmIt()
//        {
//            Console.WriteLine("Metric monitoring reports system is cold. Warming up IIS.");
//            int totalchars = 0;
//            using (var client = new WebClient())
//            {
//                for (int i = 0; i < 25; i++)
//                {
//                    var url = _geturl(i, true);
//                    if (_verbose) Console.WriteLine(url);
//                    var result = client.DownloadString(url);
//                    totalchars += result.Length;
//                    Thread.Sleep(20);
//                }
//            }
//            Thread.Sleep(3000);

//        }
//    }
//}
