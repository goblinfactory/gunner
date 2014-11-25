using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public static class BuildHelper
    {
        public static string GetVersion()
        {
            var name = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            return string.Format("{0}.{1}.{2}.{3}",
                                 name.Version.Major,
                                 name.Version.MajorRevision,
                                 name.Version.Minor,
                                 name.Version.MinorRevision).Replace("-", "1");
        }

    }
}
