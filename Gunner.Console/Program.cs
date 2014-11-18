using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
                var bootstrapper = new GunnerBootstrapper(new MachineGunFactory());
                var machineGun = bootstrapper.Bootstrap(args);
                Task.WaitAll(new[] { machineGun.Run() });
            }
            catch (Exception ex)
            {
                console.WriteLine("Error occurred, stopping.");

                if (ex.InnerException != null)
                    console.WriteLine("Error was:{0}", ex.InnerException.Message);
                else
                    console.WriteLine("Error was:{0}", ex.Message);
            }
        }

    }

}
