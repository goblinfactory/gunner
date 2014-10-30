using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public class BatchRunResult : UserRunResult {

        public decimal MemoryUsedMb { get; set; }

        public long NetworkBytesSent { get; set; }
        public long NetworkBytesRecieved { get; set; }

        public decimal NetworkMBSent
        {
            get { return (decimal) NetworkBytesSent/1048576; }
        }
        public decimal NetworkMBRecieved
        {
            get { return (decimal)NetworkBytesRecieved / 1048576; }
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
