using System.Net;

namespace NetConnectWatcher.IpHlpApi.Model
{

    public class ProcessConnectionModel
    {
        public string ProcessName { get; set; }

        public int ProcessId { get; set; }

        public ConnectionType Kind { get; set; }

        public string State { get; set; }

        public string LocalAddress { get; set; }

        public string RemoteAddress { get; set; }

    }

}
