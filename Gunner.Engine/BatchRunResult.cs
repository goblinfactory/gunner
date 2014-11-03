using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public class BatchRunResult : UserRunResult {

        public List<MetricValue> Metrics { get; set; } 
        public float RequestsPerSecond { get; set; }
        public float AverageResponseMs { get; set; }
        public decimal MemoryUsedMb { get; set; }
        public NetworkTraffic Traffic { get; set; }

        public BatchRunResult() 
        {
            Metrics = new List<MetricValue>();
        }

        public void UpdateTotals(UserRunResult ur)
        {
            Success += ur.Success;
            ur.GetStatuses().ToList().ForEach(s=> _statuses.Increment(s.Code,s.Count));
        }

        public override string ToString()
        {
            var textview = string.Join(", ", GetStatuses().Select(s => s.ToString()));
            return textview;
        }
    }
}
