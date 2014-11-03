namespace Gunner.Engine
{
    public class NetworkTraffic
    {
        public long SentBytes { get; set; }
        public long ReceivedBytes { get; set; }
        public NetworkTraffic Difference(NetworkTraffic later)
        {
            return new NetworkTraffic
                {
                    ReceivedBytes = later.ReceivedBytes - this.ReceivedBytes,
                    SentBytes = later.SentBytes - this.SentBytes
                };
        }
    }
}