using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetConnectWatcher.Binding
{
    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SetProperty<T>(ref T member, T val, [CallerMemberName] string propertyName = null, bool checkForEqual = true)
        {
            if (checkForEqual && Equals(member, val)) return;
            member = val;
            OnPropertyChanged(propertyName);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (propertyName is null) { return; }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
