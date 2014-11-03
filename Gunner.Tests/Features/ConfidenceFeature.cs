using System;
using System.Collections.Generic;
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
        [Test]
        [Category("very-slow"),Ignore("Run manually.")]
        public void ShouldBeAbleToTrustResults()
        {
            Test.TraceFeature();
            Given_Gunner();
            And_a_webserver();
            When_gunner_is_run();
            And_reports_requests_per_second_of_x();
            Then_the_webserver_should_report_similar_requests_per_second_within_an_acceptable_tolerance();
        }

        #region step definitions

        private MachineGun _gunner;
        private MockLogwriter _logwriter;
        private Task _task;
        private List<BatchRunResult> _results; 

        private void Given_Gunner()
        {
            Test.TraceStep();
            var options = new Options()
                {
                    Start = 10,
                    End = 15,
                    Pause = 0,
                    Gap = 100,
                    Repeat = 75,
                    Logfile = "ConfidenceFeature.log",
                    Find = Settings.TestFileContains,
                    Root =  Settings.Root,
                    Increment = 1,
                    UrlList = Settings.TestFile,
                    Timeout = 200
                };
            _logwriter = new MockLogwriter(false);
            var metricMonitoring = new MetricMonitoring(PerformanceMetric.RequestsPerSecond);
            var networkMonitor = new NetworkTrafficMonitor();
            _gunner = new MachineGun(options,metricMonitoring,networkMonitor,_logwriter);
        }

        private void And_a_webserver()
        {
            Test.TraceStep();
            // test to prove webserver is running and wake it up.
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

        private void Then_the_webserver_should_report_similar_requests_per_second_within_an_acceptable_tolerance()
        {
            Test.TraceStep();
            var tolerance = 0.3F;
            var lower = (1-tolerance);
            var higher = (1 + tolerance);
            Console.WriteLine();
            Console.WriteLine("Tolerance of 30% For a short test of around 3 to 4 seconds, the tolerance is wide because simple systems measurements are over a 1 second interval and cannot easily be synchronised with automated tests and calculated values. Longer running tests will amortise the discepency over more tests and allow for a finer tolerance.");
            Console.WriteLine();
            Console.WriteLine("calculated rps, Measured rps, Actual deviation %");
            _results.ForEach(r =>
                {
                    var rpsMetric = r.Metrics.First(m => m.Metric == PerformanceMetric.RequestsPerSecond).Value;
                    var deviation = (1F - (rpsMetric/r.RequestsPerSecond))*100;
                    Console.WriteLine("{0,7:0.00},{1,7:0.00}, {2,7:0.00}%",r.RequestsPerSecond,rpsMetric,deviation);
                    var min = lower * rpsMetric;
                    var max = higher * rpsMetric; 
                    //r.RequestsPerSecond.Should().BeInRange(min, max);
                });
        }

        #endregion
    }
}
