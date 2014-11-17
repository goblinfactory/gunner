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
    public class Bootstrapper
    {
        private readonly ILogWriter _console;

        public Bootstrapper(ILogWriter console)
        {
            _console = console;
        }

        public Bootstrapper()
        {
            _console = new ConsoleWriter();
        }

        public void Start(params string[] args)
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
                _console.WriteLine("Error occurred, stopping.");

                if (ex.InnerException != null)
                    _console.WriteLine("Error was:{0}", ex.InnerException.Message);
                else
                    _console.WriteLine("Error was:{0}", ex.Message);
                Usage();
            }

        }

        private void Usage()
        {
            _console.WriteLine("mechanic listen");
            _console.WriteLine(new ListenSubOptions().GetUsage());
            _console.WriteLine("");
            _console.WriteLine("mechanic send");
            _console.WriteLine(new SendSubOptions().GetUsage());
        }

        private void Run(string[] args)
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
                    using (var messenger = new SimpleSocketTester("127.0.0.1", lo.Port, new [] { _console }))
                        messenger.RecieveMessage(lo.Response, lo.Cnt);
                    break;
                case Verb.send:
                    var so = (SendSubOptions)invokedVerbInstance;
                    using (var messenger = new SimpleSocketTester("127.0.0.1", so.Port, new[] { _console }))
                        messenger.SendMessages(so.Wait, so.Messages);
                    break;
                default:throw new ArgumentOutOfRangeException("invokedVerb",invokedVerb,"not supported.");
            }
        }

    }
}
