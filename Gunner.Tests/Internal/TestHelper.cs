using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Gunner.Tests.Internal
{
    public static class Test
    {
        public static void TraceStep()
        {
            Console.WriteLine(new StackTrace().GetFrame(1).GetMethod().Name.Replace("_"," "));
        }

        public static void TraceFeature()
        {
            var name = new StackTrace().GetFrame(1).GetMethod().Name.Replace("_", " ");
            Console.WriteLine(name);
            Console.WriteLine(new string('-',name.Length));
        }

    }
}
