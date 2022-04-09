using System;
using System.Threading;

namespace NetConnectWatcher.Binding
{
    public class ViewModelBase : BindableBase
    {
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;

        protected void InvokeOnMainThreadSync(Action action)
        {
            if (synchronizationContext != null && SynchronizationContext.Current != synchronizationContext)
                synchronizationContext.Send(_ => action(), null);
            else
                action();
        }

        protected void InvokeOnMainThreadASync(Action action)
        {
            if (synchronizationContext != null && SynchronizationContext.Current != synchronizationContext)
                synchronizationContext.Post(_ => action(), null);
            else
                action();
        }

        protected bool IsMainThread => SynchronizationContext.Current == synchronizationContext;

    }
}
