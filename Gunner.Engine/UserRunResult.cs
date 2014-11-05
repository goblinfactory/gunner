using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public class UserRunResult
    {
        public UserRunResult()
        {
            _statuses = new Dictionary<int, int>();
        }

        protected virtual Dictionary<int,int> _statuses { get; set; }
        public int Success { get; set; }
        public int Fail { get; set; }
        public int Total
        {
            get { return Success + Fail; }            
        }

        public void UpdateTotals(DownloadResult dr)
        {
            if (dr.Success)
                Success++;
            else
                _statuses.Increment(dr.ErrorCode ?? 0);
        }
    
        public Status[] GetStatuses()
        {
            return _statuses.Select(s => new Status() {Code = s.Key, Count = s.Value}).ToArray();
        }
    }


    public class Status
    {
        public int Code { get; set; }
        public int Count { get; set; }
        public override string ToString()
        {
            return string.Format("http-{0}:{1}", Code, Count);
        }
    }

    public class DownloadResult
    {
        public int? ErrorCode { get; set; }
        public bool Success { get; set; }
    }

}
