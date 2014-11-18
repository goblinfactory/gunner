using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gunner.Engine;

namespace Gunner.Console
{
    public class GunnerBootstrapper
    {
        private readonly IMachineGunFactory _machineGunFactory;

        public GunnerBootstrapper(IMachineGunFactory machineGunFactory)
        {
            _machineGunFactory = machineGunFactory;
        }

        public MachineGun Bootstrap(string[] args)
        {
            var options = GetGunnerOptions(args);
            var machineGun = _machineGunFactory.CreateMachineGun(options);
            return machineGun;
        }

        private BatchOptions GetGunnerOptions(string[] args)
        {
            var options = new BatchOptions();
            if (!CommandLine.Parser.Default.ParseArguments(args, options)) throw new ApplicationException("Invalid arguments.");
            if (options.Csv.HasText() && options.UrlList.HasText()) throw new ApplicationException("You cannot provide a value for csv as well as urls, only 1 option can be selected.");
            if (options.End > 5000) throw new ApplicationException("Maximum value atmo for users is 5000. This is due to socket connection limit. Later will detect if user can support more, if not show message how to tell OS you want more.");
            if (!string.IsNullOrWhiteSpace(options.Logfile))
            {
                var path = Path.Combine(Environment.CurrentDirectory, options.Logfile);
                File.Create(path).Close();
                options.Logfile = path;
            }
            return options;
        }



    }
}
