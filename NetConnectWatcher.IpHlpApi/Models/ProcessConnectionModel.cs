using System.Net;

namespace NetConnectWatcher.IpHlpApi.Model
{
    public interface IDeepCopy<T>
    {
        T DeepCopy();
    }

    public class ProcessConnectionModel : IDeepCopy<ProcessConnectionModel>
    {
        public string ProcessName { get; set; }

        public int ProcessId { get; set; }

        public ConnectionType Kind { get; set; }

        public string State { get; set; }

        public string LocalAddress { get; set; }

        public string RemoteAddress { get; set; }

        public ProcessConnectionModel DeepCopy()
        {
            ProcessConnectionModel other = (ProcessConnectionModel)this.MemberwiseClone();
            other.ProcessName = string.Copy(ProcessName);
            other.State = string.Copy(State);
            other.LocalAddress = string.Copy(LocalAddress);
            other.RemoteAddress = string.Copy(RemoteAddress);
            return other;
        }
    }

}
