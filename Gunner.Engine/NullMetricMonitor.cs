using System.Collections.Generic;

namespace Gunner.Engine
{
    public class NullMetricMonitor : IMetricMonitoring
    {
        private readonly List<MetricValue> _metrics = new List<MetricValue>(); 

        public List<MetricValue> ReadMetrics() {
            return _metrics;
        }

        public bool SystemBeingMonitoredIsCold { get { return false;  } }
    }
}