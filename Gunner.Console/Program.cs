using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gunner.Engine;
using console = System.Console;

namespace Gunner.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Run(args);
            }
            catch (Exception ex)
            {
                console.WriteLine("Error occurred, stopping.");
                
                if (ex.InnerException!=null)
                    console.WriteLine("Error was:{0}",ex.InnerException.Message);
                else
                    console.WriteLine("Error was:{0}", ex.Message);
            }

        }

        private static void Run(string[] args)
        {
            var options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options)) return;
            if (!string.IsNullOrWhiteSpace(options.Logfile))
            {
                var path = Path.Combine(Environment.CurrentDirectory, options.Logfile);
                console.WriteLine("env:{0} | logfile:{1} | path:{2}", Environment.CurrentDirectory, options.Logfile, path);
                File.Create(path).Close();
                options.LogPath = path;
            }
            var mg = new MachineGun(options);
            mg.Run();                
        }

    }
}
