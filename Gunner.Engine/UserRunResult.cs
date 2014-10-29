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

        private Dictionary<int,int> _statuses { get; set; }
        public int Success { get; set; }
        public int Fail { get; set; }

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


    public static class DictionaryHelper
    {
        public static void Increment(this Dictionary<int, int> dictionary, int key)
        {
            int i;
            dictionary.TryGetValue(key, out i);
            dictionary[key] = i + 1;
        }
    }

    public class Status
    {
        public int Code { get; set; }
        public int Count { get; set; }
    }

    public class DownloadResult
    {
        public int? ErrorCode { get; set; }
        /// <summary>
        /// result was 2xx and result string contained correct match.
        /// </summary>
        public bool Success { get; set; }
    }

}
