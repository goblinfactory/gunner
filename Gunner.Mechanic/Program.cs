using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gunner.Engine;
using Gunner.Engine.Mechanic;

namespace Gunner.Mechanic
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Main(args, new ConsoleWriter());
        }

        public static void Main(string[] args, ILogWriter writer)
        {
            new ConsoleHelper().RunCatchWriteExceptionsToConsole(args,Usage,Run);
        }

        private static void Usage()
        {
            Console.WriteLine("mechanic listen");
            Console.WriteLine(new ListenSubOptions().GetUsage());
            Console.WriteLine();

            Console.WriteLine("mechanic send");
            Console.WriteLine(new SendSubOptions().GetUsage());
        }

        private static void Run(string[] args)
        {

            string invokedVerb =null;
            object invokedVerbInstance = null;

            var options = new MechanicOptions();
            if (!CommandLine.Parser.Default.ParseArguments(args, options, (verb, subOptions) => {
                    invokedVerb = verb;
                    invokedVerbInstance = subOptions;
            }))
            {
                Usage();
                return;
            }
                
            switch (invokedVerb)
            {
                case Verb.listen:
                    var lo = (ListenSubOptions)invokedVerbInstance;
                    using (var simpleSocketTester = new SimpleSocketTester(lo.Server, lo.Port))
                        simpleSocketTester.RecieveMessage(lo.Response, lo.Cnt);
                    break;
                case Verb.send:
                    var so = (SendSubOptions)invokedVerbInstance;
                    using (var simpleSocketTester = new SimpleSocketTester(so.Server, so.Port))
                        simpleSocketTester.SendMessages(so.Wait, so.Messages);
                    break;
                default:throw new ArgumentOutOfRangeException("invokedVerb",invokedVerb,"not supported.");
            }
        }

    }
}
