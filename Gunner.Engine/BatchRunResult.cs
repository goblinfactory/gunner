using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public class BatchRunResult : UserRunResult {

        public DateTime TimeStart { get; set; }
        public DateTime TimeStop { get; set; }
        public long DurationMs { get; set; }

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

    public static class BatchRunResultExtensions
    {
        /// <summary>
        /// Skip all the batches that are not at least (x) ms elapsed duration from the start of the first batch. (So that we can ensure that any averaging done over a range, e.g. system monitoring, does not include any averaging over any period where our testing had not yet started.
        /// </summary>
        public static IEnumerable<BatchRunResult> SkipUntil(this IEnumerable<BatchRunResult> src, int msFromFirstBatchStartToSkip)
        {
            //NB! need a try catch, if skipping past end of list! 
            var firstStart = src.First().TimeStart;
            var enumerator = src.GetEnumerator();
            var current = enumerator.Current;
            while (current.TimeStart < firstStart.AddMilliseconds(msFromFirstBatchStartToSkip))
            {
                enumerator.MoveNext();
            }
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }
    }

}
