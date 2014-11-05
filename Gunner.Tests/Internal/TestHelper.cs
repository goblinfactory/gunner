using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gunner.Tests.Internal
{
    public static class Test
    {

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void TraceFeature()
        {
            var name = new StackTrace().GetFrame(1).GetMethod().Name.Replace("_", " ");
            Console.WriteLine(name);
            Console.WriteLine(new string('-',name.Length));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void TraceStep()
        {
            var name = new StackTrace().GetFrame(1).GetMethod().Name.Replace("_", " ");
            Console.WriteLine(name);
        }

    }
}
