using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Gunner.Tests.Internal;
using NUnit;
using NUnit.Framework;
using NetMQ;

namespace Gunner.Tests.Features
{
    [SetUpFixture]
    public class _FeatureBootstrapper
    {
        private static bool _websiteWarmedUp = false;

        [SetUp,Category("slow")]
        public void RunOnceWarmupSystemForTesting()
        {
            Console.WriteLine("warming up website");
            WarmupWebsite();
        }

        public static void WarmupIISMetrics()
        {
            try
            {
                using (var pc = new PerformanceCounter())
                {
                    pc.CategoryName = @"W3SVC_W3WP";
                    pc.InstanceName = @"_Total";
                    pc.CounterName = @"Requests / Sec";
                    float? result = null;
                    Console.WriteLine("warming up performance counter; Current rps is:{0}",pc.NextValue());
                    //Action action2 = () => result = pc.NextValue();
                    //action2.ShouldNotThrow();
                    //result.HasValue.Should().BeTrue();
                }
            }
            catch (InvalidOperationException ioex)
            {               
                if (ioex.Message.Contains("Category"))
                    throw new InvalidOperationException("If you see this message, for now, simply re-run the unit tests. Looks like VS or R# has a lock on IIS or something causing performance counters not to be created first time around.", ioex);
                throw;
            }
        }

        public static void WarmupWebsite()
        {
            if (_websiteWarmedUp) return;
            using (var client = new WebClient())
            {
                var result = client.DownloadString(Settings.Root + Settings.TestFile);
                result.Should().Contain(Settings.TestFileContains);
                WarmupIISMetrics();
                _websiteWarmedUp = true;
            }
        }
    }
}
