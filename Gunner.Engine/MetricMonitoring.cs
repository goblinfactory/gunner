using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public class MetricMonitor
    {
        public PerformanceCounter Monitor { get; set; }
        public PerformanceMetric Metric { get; set; }
    }

    public class MetricMonitoring : IMetricMonitoring, IDisposable
    {
        private List<MetricMonitor> _monitors;
        /// <summary>
        /// Returns true if the performance monitors were not able to read the first set of throw away values immediately. 
        /// This often happens if you are monitoring something that needs to be warmed up for the performance counter
        /// category to exist first, for example IIS.
        /// </summary>
        public bool SystemBeingMonitoredIsCold { get; set; }
        public MetricMonitoring(params PerformanceMetric[] metrics)
        {
            _monitors = metrics.Select(m => m.ToCounter()).ToList();
            try
            {
                _monitors.ForEach(m => m.Monitor.NextValue());
                SystemBeingMonitoredIsCold = false;
            }
            catch (InvalidOperationException)
            {
                SystemBeingMonitoredIsCold = true;
            }
        }

        public List<MetricValue> ReadMetrics()
        {
            // see -> http://msdn.microsoft.com/en-us/library/xb29hack(v=vs.90).aspx ( nextValue vs NextSample()
            //NB! the category may not yet exist if the thing being monitored has not been
            //NB! ...  running long enough (1 second often) for the first period to have elapsed and for
            //NB! ... the first value to arrive, causing the category to exist!
            return _monitors.Select(m => new MetricValue { Metric = m.Metric, Value = m.Monitor.NextValue() }).ToList();
        }

        bool _disposed;

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MetricMonitoring() {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) { _monitors.ForEach(m=>m.Monitor.Dispose()); }
            _disposed = true;
        }

    }

    public enum PerformanceMetric
    {
        RequestsPerSecond, TotalHttpRequestsServed
    };

    public static class MetricMonitorFactory
    {
        public static MetricMonitor ToCounter(this PerformanceMetric metric)
        {
            var pc = new PerformanceCounter();
            pc.ReadOnly = true;
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
                    throw new ArgumentOutOfRangeException("metric", metric.ToString());
            }
            pc.BeginInit();
            return new MetricMonitor() { Metric = metric, Monitor = pc };

        }
    }

}
