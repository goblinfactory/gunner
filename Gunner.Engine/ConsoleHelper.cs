using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public class ConsoleHelper
    {
        public void RunCatchWriteExceptionsToConsole(string[] args, Action usage, Action<string[]> run)
        {
            if (args == null || args.Length == 0)
            {
                usage();
                return;
            }
            try
            {
                run(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred, stopping.");

                if (ex.InnerException != null)
                    Console.WriteLine("Error was:{0}", ex.InnerException.Message);
                else
                    Console.WriteLine("Error was:{0}", ex.Message);
                usage();
            }
        }

    }
}
