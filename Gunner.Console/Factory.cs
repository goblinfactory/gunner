using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gunner.Engine;

namespace Gunner.Console
{
    public class Factory
    {
        public MachineGun CreateMachineGun(Options options, IMetricMonitoring metricMonitoring = null)
        {
            // dependancies
            // ============
            var urls =              new UrlReader(options).ReadUrls(Environment.CurrentDirectory);
            ILogWriter logwriter    = new LogWriter(options.Logfile);
            var trafficMonitor      = new NetworkTrafficMonitor();
            var errorLogger         = new ErrorLogger("notused-yet!",false);
            var downloader          = new Downloader(errorLogger);
            var reporter            = new Reporter(options, logwriter);
            
            var machineGun = new MachineGun(reporter,downloader,options, urls, trafficMonitor, metricMonitoring);
            return machineGun;
        }
    }
}
