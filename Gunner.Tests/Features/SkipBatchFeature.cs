using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gunner.Engine;
using Gunner.Tests.Internal;
using Gunner.Tests.Mocks;
using NUnit.Framework;

namespace Gunner.Tests.Features
{
    [TestFixture]
    class SkipBatchFeature
    {

        // =========================================================================================================
        //
        //                                                  REQUIREMENTS
        //
        // =========================================================================================================
        
        [Test, Ignore("still to do.")]
        [Category("fast")]
        [TestCase(5, 2, 3)]
        [TestCase(7, 0, 7)]
        [TestCase(4, -1, 4)]
        public void SkippedBatchesShouldNotBeLogged(int batches, int skipBatches, int cntLogged)
        {
            // given a gunner script that will result in 5 batches
            
            Given_Gunner_options_that_will_result_in_X_batches(batches);
            Given_option_skip_batches_of(2);
            When_Gunner_is_run();
            // 5 batches should be diplayed to the screen
            // the first 2 batches will end with ** SKIPPED
            // the last 3 batches will not end with **SKIPPED
            // the skipped batches should not be logged
        }




        [Test]
        public void SkippedBatchesShouldNotCountTowardsStatisticResults()
        {

        }

        // =========================================================================================================
        //
        //                                                   BINDINGS
        //
        // =========================================================================================================

        #region bindings

        //private MockLogwriter _logwriter;

        private BatchOptions _batchOptions;

        private void Given_Gunner_options_that_will_result_in_X_batches(int batches)
        {
            Test.TraceStep(batches);
            int increment = 5;
            int start = 10;
            _batchOptions = new BatchOptions
            {
                Start = 10,
                End = start + (increment*(batches-1)),
                Pause = 0,
                Gap = 0,
                Repeat = 5,
                Logfile = "SkipBatchFeature.log",
                Find = Settings.TestFileContains,
                Root = Settings.Root,
                Increment = increment,
                UrlList = Settings.TestFile,
                Timeout = 3,
                Verbose = false
            };
        }

        private void Given_option_skip_batches_of(int i)
        {
            Test.TraceStep(i);
            _batchOptions.SkipBatches = i;
        }

        private void When_Gunner_is_run()
        {
            Test.TraceStep();
            var logwriter = new MockLogWriter(false);
            var metricMonitoring = new NullMetricMonitor();
            var trafficMonitor = new MockTrafficMonitor(100);
            var urls = new[] { "file1.json", "file2.json" };
            var downloader = new MockDownloader(new DownloadResult { ErrorCode = null, Success = true });
            var reporter = new BatchReporter(_batchOptions, logwriter);
            var gunner = new MachineGun(reporter,downloader,_batchOptions, urls, trafficMonitor, metricMonitoring);
            Task.WaitAll(gunner.Run());
        }

        #endregion

    }

      
}
