using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public static class MemoryHelper
    {
        public static decimal GetPeakWorkingSetKb()
        {
            return Process.GetCurrentProcess().PeakWorkingSet64/(1048576M);
        }
    }
}
