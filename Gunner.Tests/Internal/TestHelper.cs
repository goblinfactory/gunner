using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gunner.Tests.Internal
{
    public static class Test
    {
        //NB! this doesnt appear to work when doing a release build, seems to inline these methods into the caller causing title to be for the main test class, and not for the method! huh!
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void TraceFeature(params string[] parms)
        {
            var name = new StackTrace().GetFrame(1).GetMethod().Name.Replace("_", " ");
            Console.WriteLine(name);
            Console.WriteLine(new string('-',name.Length));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void TraceStep(object value =null)
        {
            var name = new StackTrace().GetFrame(1).GetMethod().Name.Replace("_", " ");
            Console.WriteLine(name + " " + value ?? "");
        }

    }
}
