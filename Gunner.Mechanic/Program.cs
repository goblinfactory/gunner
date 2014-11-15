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
            if (args == null || args.Length == 0) 
            { 
                Usage();
                return; 
            }
            try
            {
                Run(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred, stopping.");

                if (ex.InnerException != null)
                    Console.WriteLine("Error was:{0}", ex.InnerException.Message);
                else
                    Console.WriteLine("Error was:{0}", ex.Message);
                Usage();
            }

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
                    using (var messenger = new SimpleSocketMessenger("127.0.0.1", lo.Port))
                        messenger.RecieveMessage(lo.Response, lo.Cnt);
                    break;
                case Verb.send:
                    var so = (SendSubOptions)invokedVerbInstance;
                    using (var messenger = new SimpleSocketMessenger("127.0.0.1", so.Port))
                        messenger.SendMessages(so.Wait, so.Messages);
                    break;
                default:throw new ArgumentOutOfRangeException("invokedVerb",invokedVerb,"not supported.");
            }
        }

    }
}
