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
    public class ShouldBeAbleToTrustResultsForTests
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
        [Category("slow")]
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
        //                                                BINDINGS
        //
        // =========================================================================================================

        #region step definitions


        private MachineGun _gunner;
        private MockLogWriter _logwriter;
        private Task _task;
        private List<BatchRunResult> _results;

        private void Given_Gunner(int repeat)
        {
            Test.TraceStep();
            var options = new BatchOptions()
                {
                    Start = 10,
                    End = 30,
                    Pause = 0,
                    Gap = 20,
                    Repeat = repeat,
                    Logfile = "ConfidenceFeature.log",
                    Find = Settings.TestFileContains,
                    Root =  Settings.Root,
                    Increment = 10,
                    UrlList = Settings.TestFile,
                    Timeout = 200,
                    Verbose = false
                };
            _logwriter = new MockLogWriter(false);
            var metricMonitoring = new MetricMonitoring(PerformanceMetric.RequestsPerSecond);
            var trafficMonitor = new NetworkTrafficMonitor();
            var urls = new UrlReader(options).ReadUrls(Environment.CurrentDirectory);
            var errorLogger = new MockErrorLogger(true);
            var downloader = new Downloader(errorLogger);
            var reporter = new BatchReporter(options, _logwriter);
            _gunner = new MachineGun(reporter, downloader, options, urls, trafficMonitor, metricMonitoring);
        }

        private void Given_a_webserver()
        {
            Test.TraceStep();
            _FeatureBootstrapper.WarmupWebsite();
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
                    var min = lower * r.RequestsPerSecond;
                    var max = higher * r.RequestsPerSecond; 
                    r.RequestsPerSecond.Should().BeInRange(min, max);
                });
        }

        #endregion
    }

}
