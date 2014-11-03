using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public interface ITrafficMonitor 
    {
        NetworkTraffic ReadTrafficSinceMonitoringStarted();
        void StartMonitoring();
    }

    public class NetworkTrafficMonitor : ITrafficMonitor
    {
        private readonly List<IPv4InterfaceStatistics> _activeNicStats;

        private NetworkTraffic _baseline = null;
        public NetworkTraffic Baseline { get { return _baseline; } }

        public NetworkTrafficMonitor()
        {
            _activeNicStats = NetworkInterface
            .GetAllNetworkInterfaces()
            .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
            .Select(ni => ni.GetIPv4Statistics())
            .ToList();
        }

        private NetworkTraffic GetTraffic()
        {
            return new NetworkTraffic
            {
                SentBytes = _activeNicStats.Sum(x => x.BytesSent),
                ReceivedBytes = _activeNicStats.Sum(x => x.BytesReceived)
            };
        }

        public void StartMonitoring()
        {
            _baseline = GetTraffic();
        }

        public NetworkTraffic ReadTrafficSinceMonitoringStarted()
        {
            if (_baseline == null) throw new ApplicationException("cannot read traffic before traffic monitoring is started. Please .Start() monitoring when you wwant to estblish a baseline.");
            return GetTraffic().Difference(_baseline);
        }


    }
}
