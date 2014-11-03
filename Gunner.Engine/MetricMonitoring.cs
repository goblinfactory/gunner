using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public class MetricMonitor
    {
        public PerformanceCounter Monitor { get; set; }
        public PerformanceMetric Metric { get; set; }
    }

    public class MetricMonitoring : IMetricMonitoring
    {
        private List<MetricMonitor> _monitors;
 
        public MetricMonitoring(params PerformanceMetric[] metrics)
        {
            _monitors = metrics.Select(m => m.ToCounter()).ToList();
            _monitors.ForEach(m=>m.Monitor.NextValue());
        }


        public List<MetricValue> ReadMetrics()
        {
            // see -> http://msdn.microsoft.com/en-us/library/xb29hack(v=vs.90).aspx ( nextValue vs NextSample()
            return _monitors.Select(m => new MetricValue {Metric = m.Metric, Value = m.Monitor.NextValue()}).ToList();
        }
    }

    public enum PerformanceMetric
    {
        RequestsPerSecond,TotalHttpRequestsServed
    };

    public static class MetricMonitorFactory
    {
        public static MetricMonitor ToCounter(this PerformanceMetric metric)
        {
            var pc = new PerformanceCounter();
            pc.CategoryName = @"W3SVC_W3WP";
            pc.InstanceName = @"_Total";

            switch (metric)
            {
                // NB! if not running as administrator, will return zero!

                case PerformanceMetric.RequestsPerSecond:
	                pc.CounterName = @"Requests / Sec";
                    break;
                case PerformanceMetric.TotalHttpRequestsServed:
                    pc.CounterName = @"Total HTTP Requests Served";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("metric",metric.ToString());
            }
            return new MetricMonitor() { Metric = metric, Monitor = pc };

        }
    }

}
