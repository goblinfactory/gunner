using Gunner.Engine;

namespace Gunner.Tests.Mocks
{
    public class MockTrafficMonitor : ITrafficMonitor
    {
        private readonly int _bytesSentRecievedPerBatch;

        public MockTrafficMonitor(int bytesSentRecievedPerBatch)
        {
            _bytesSentRecievedPerBatch = bytesSentRecievedPerBatch;
        }

        public NetworkTraffic ReadTrafficSinceMonitoringStarted()
        {
            return new NetworkTraffic()
                {
                    ReceivedBytes = _bytesSentRecievedPerBatch,
                    SentBytes = _bytesSentRecievedPerBatch
                };
        }

        public void StartMonitoring()
        {
        }
    }
}
