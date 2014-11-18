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
        public MachineGun CreateMachineGun(BatchOptions batchOptions, IMetricMonitoring metricMonitoring = null)
        {
            // dependancies
            // ============
            var urls =              new UrlReader(batchOptions).ReadUrls(Environment.CurrentDirectory);
            ILogWriter logwriter    = new LogWriter(batchOptions.Logfile);
            var trafficMonitor      = new NetworkTrafficMonitor();
            var errorLogger         = new ErrorLogger(batchOptions.ErrorLogfile,batchOptions.LogErrors);
            var downloader          = new Downloader(errorLogger);
            var reporter            = new Reporter(batchOptions, logwriter);
            
            var machineGun = new MachineGun(reporter,downloader,batchOptions, urls, trafficMonitor, metricMonitoring);
            return machineGun;
        }
    }
}
