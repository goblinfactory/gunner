using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Gunner.Tests.Features
{
    [TestFixture]
    class SkipBatchFeature
    {
        [Test]
        [Category("fast")]
        [TestCase(5, 2, 3)]
        [TestCase(7, 0, 7)]
        [TestCase(4, -1, 4)]
        public void SkippedBatchesShouldNotBeLogged(int batches, int skipBatches, int cntLogged)
        {
            // given a gunner script with 5 batches
            // and skipbatch = 3
            // when the script is run
            // only 3 batches should be logged
        }


    

        [Test]
        public void SkippedBatchesShouldNotCountTowardsStatisticResults()
        {

        }


        #region step definitions

        private void Given_Gunner(int repeat)
        {
            Test.TraceStep();
            var options = new Options()
            {
                Start = 50,
                End = 300,
                Pause = 0,
                Gap = 20,
                Repeat = repeat,
                Logfile = "ConfidenceFeature.log",
                Find = Settings.TestFileContains,
                Root = Settings.Root,
                Increment = 50,
                UrlList = Settings.TestFile,
                Timeout = 200,
                Verbose = false
            };
            // should I use a builder?
            _logwriter = new MockLogwriter(false);
            var metricMonitoring = new MetricMonitoring(PerformanceMetric.RequestsPerSecond);
            var trafficMonitor = new NetworkTrafficMonitor();
            var urls = new UrlReader(options).ReadUrls(Environment.CurrentDirectory);
            //NB! Move errors and lastflush into Downloader class! or into errorlogger that's passed to downloader
            var downloader = new Downloader(_errors, ref _lastFlush);
            _gunner = new MachineGun(downloader, options, _logwriter, urls, trafficMonitor, metricMonitoring);
        }

        #endregion
    }


}
