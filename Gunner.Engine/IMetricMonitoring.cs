using System.Collections.Generic;

namespace Gunner.Engine
{

    public class MetricValue
    {
        public PerformanceMetric Metric { get; set; }
        public float Value { get; set; }
    }

    public interface IMetricMonitoring
    {
         List<MetricValue> ReadMetrics();
    }
}