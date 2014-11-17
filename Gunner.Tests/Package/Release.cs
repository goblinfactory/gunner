using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gunner.Tests.Build
{
    public static class Release
    {
        public static string CurrentBuild
        {
            get
            {
#if DEBUG
                return "DEBUG";
#else
                return "RELEASE";
#endif
            }
        }
    }
}
