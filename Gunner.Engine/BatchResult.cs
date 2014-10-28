using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public class BatchResult
    {
        public BatchResult() { Statuses = new List<Status>();}
        public List<Status> Statuses { get; set; }
        public int Success { get; set; }
        public int Fail { get; set; }
    
    }

    public class Status
    {
        public int Code { get; set; }
        public int Count { get; set; }
    }
}
