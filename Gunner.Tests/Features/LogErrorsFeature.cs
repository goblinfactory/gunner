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
    class LogErrorsFeature
    {

        // =========================================================================================================
        //
        //                                                  REQUIREMENTS
        //
        // =========================================================================================================

        [Test]
        [Category("fast")]
        public void Error404ShouldBeLoggedToFileIfErrorLoggingEnabled()
        {
            Test.TraceFeature();
            Given_a_gunner_script_that_will_run_once_through_a_csv_file();
            And_a_csv_file_containing_urls_that_exist_as_well_as_some_that_dont();
            When_the_script_is_run();
            Then_one_404_error_should_be_logged_to_the_error_file_per_invalid_url();
        }

        // =========================================================================================================
        //
        //                                                   BINDINGS
        //
        // =========================================================================================================

        #region bindings


        private void Given_a_gunner_script_that_will_run_once_through_a_csv_file()
        {
            Test.TraceStep();
        }

        private void And_a_csv_file_containing_urls_that_exist_as_well_as_some_that_dont()
        {
            Test.TraceStep();
        }

        private void When_the_script_is_run()
        {
            Test.TraceStep();
        }

        private void Then_one_404_error_should_be_logged_to_the_error_file_per_invalid_url()
        {
            Test.TraceStep();
        }


 


        //private MockLogwriter _logwriter;

        private Options _options;

        private void Given_Gunner_options_that_will_result_in_X_batches(int batches)
        {
            Test.TraceStep(batches);
            int increment = 5;
            int start = 10;
            _options = new Options
            {
                Start = 10,
                End = start + (increment * (batches - 1)),
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
            _options.SkipBatches = i;
        }

        private void When_Gunner_is_run()
        {
            Test.TraceStep();
            var logwriter = new MockLogWriter(false);
            var metricMonitoring = new NullMetricMonitor();
            var trafficMonitor = new MockTrafficMonitor(100);
            var urls = new[] { "file1.json", "file2.json" };
            var downloader = new MockDownloader(new DownloadResult { ErrorCode = null, Success = true });
            var reporter = new Reporter(_options, logwriter);
            var gunner = new MachineGun(reporter, downloader, _options, urls, trafficMonitor, metricMonitoring);
            Task.WaitAll(gunner.Run());
        }

        #endregion

    }


}
