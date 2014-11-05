using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Gunner.Engine;
using Gunner.Tests.Build;
using Gunner.Tests.Internal;
using Gunner.Tests.Mocks;
using NUnit.Framework;

namespace Gunner.Tests.Features
{
    [TestFixture]
    public class ConfidenceFeature
    {
        // =========================================================================================================
        //
        //                                                  REQUIREMENTS
        //
        // =========================================================================================================

        [Test]
        [Category("very-slow"),Ignore("Run manually.")]
        public void ShouldBeAbleToTrustResultsForTestsWithLargeSampleSizes()
        {
            // note: where large = > 5000 samples, and time > 20 seconds
            Test.TraceFeature();
            Given_a_webserver();
            Given_Gunner(repeat:75);
            When_gunner_is_run();
            And_reports_requests_per_second_of_x();
            Then_the_webserver_should_report_similar_requests_per_second_within_an_acceptable_tolerance(0.02F);
        }

        [Test]
        [Category("very-slow"), Ignore("Run manually.")]
        public void ShouldBeAbleToTrustResultsForTestsWithMediumSampleSizes()
        {
            // note: where medium = > 750 samples, and time > 5 seconds
            Test.TraceFeature();
            Given_a_webserver();
            Given_Gunner(20);
            When_gunner_is_run();
            And_reports_requests_per_second_of_x();
            Then_the_webserver_should_report_similar_requests_per_second_within_an_acceptable_tolerance(0.04F);
        }

        [Test]
        [Category("slow"), Ignore("Run manually.")]
        public void ShouldBeAbleToTrustResultsForTestsWithSmallSampleSizes()
        {
            // note: where small = > 200 samples, and time > 5 seconds
            Test.TraceFeature();
            Given_a_webserver();
            Given_Gunner(5);
            When_gunner_is_run();
            And_reports_requests_per_second_of_x();
            Then_the_webserver_should_report_similar_requests_per_second_within_an_acceptable_tolerance(0.1F);
        }


        // =========================================================================================================
        //
        //                                                STEP DEFINITIONS
        //
        // =========================================================================================================

        #region step definitions


        private MachineGun _gunner;
        private MockLogwriter _logwriter;
        private Task _task;
        private List<BatchRunResult> _results;
        private DateTime _lastFlush = DateTime.Now;
        private readonly List<string> _errors = new List<string>();

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
                    Root =  Settings.Root,
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
            _gunner = new MachineGun(downloader, options,_logwriter, urls, trafficMonitor, metricMonitoring);
        }

        private void Given_a_webserver()
        {
            Test.TraceStep();
            var client = new WebClient();
            var result = client.DownloadString(Settings.Root + Settings.TestFile);
            result.Should().Contain(Settings.TestFileContains);
        }

        private void When_gunner_is_run()
        {
            Test.TraceStep();
            Console.WriteLine("");
            _task = _gunner.Run();
            Task.WaitAll(_task);
            _results = _gunner.ReadResults();
        }

        private void And_reports_requests_per_second_of_x()
        {
            Test.TraceStep();
            _logwriter.ReadLines().Count().Should().BeGreaterThan(1);
            _results.ForEach(r=> r.RequestsPerSecond.Should().BeGreaterThan(1));
        }

        private void Then_the_webserver_should_report_similar_requests_per_second_within_an_acceptable_tolerance(float tolerance)
        {
            Test.TraceStep();
            var lower = (1-tolerance);
            var higher = (1 + tolerance);
            Console.WriteLine();
            Console.WriteLine("Skipping first result and checking deviation no more than {0:0.00}%.",tolerance * 100);
            Console.WriteLine("calculated rps, Measured rps, Actual deviation %");
            _results.Skip(1).ToList().ForEach(r =>
                {
                    var rpsMetric = r.Metrics.First(m => m.Metric == PerformanceMetric.RequestsPerSecond).Value;
                    var deviation = (1F - (rpsMetric/r.RequestsPerSecond))*100;
                    Console.WriteLine("{0,7:0.00},{1,7:0.00}, {2,7:0.00}%",r.RequestsPerSecond,rpsMetric,deviation);
                    var min = lower * rpsMetric;
                    var max = higher * rpsMetric; 
                    r.RequestsPerSecond.Should().BeInRange(min, max);
                });
        }

        #endregion
    }

}
