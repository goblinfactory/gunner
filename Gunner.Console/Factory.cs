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
        private List<string> _errrors = new List<string>();
        private DateTime _lastFlush = DateTime.Now;

        public MachineGun CreateMachineGun(Options options, IMetricMonitoring metricMonitoring = null)
        {
            var urls = new UrlReader(options).ReadUrls(Environment.CurrentDirectory);
            ILogWriter logwriter = new LogWriter(options.Logfile);
            var trafficMonitor = new NetworkTrafficMonitor();
            var downloader = new Downloader(_errrors, ref _lastFlush);
            var machineGun = new MachineGun(downloader,options, logwriter, urls, trafficMonitor, metricMonitoring);
            return machineGun;
        }
    }
}
