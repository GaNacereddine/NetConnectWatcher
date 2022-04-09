namespace NetConnectWatcher.Abstractions
{
    public interface IConnectionsFilterer
    {
        void FilterConnectionsByPid(int processId);
    }
}
