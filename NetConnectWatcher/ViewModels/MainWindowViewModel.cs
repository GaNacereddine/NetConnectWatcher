using NetConnectWatcher.Abstractions;
using NetConnectWatcher.Binding;
using NetConnectWatcher.IpHlpApi;
using NetConnectWatcher.IpHlpApi.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace NetConnectWatcher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IConnectionsFilterer
    {

        CancellationTokenSource _cancellationSource = new CancellationTokenSource();
        CancellationToken _cancellationToken => _cancellationSource.Token;

        public MainWindowViewModel()
        {
            StartMonitoringCommand = new RelayCommand(StartMonitoringCommandExecute);
            StopMonitoringCommand = new RelayCommand(StopMonitoringCommandExecute);   
        }

        public RelayCommand StartMonitoringCommand { get; }
        public RelayCommand StopMonitoringCommand { get; }

        private List<ProcessConnectionModel> _internalConnectionList = new List<ProcessConnectionModel>();

        private ObservableCollection<ProcessConnectionModel> _processList = new ObservableCollection<ProcessConnectionModel>();
        public ObservableCollection<ProcessConnectionModel> ProcessList
        {
            get => _processList;
            set => SetProperty(ref _processList, value);
        }

        private ObservableCollection<ProcessConnectionModel> _filteredConnectionList = new ObservableCollection<ProcessConnectionModel>();
        public ObservableCollection<ProcessConnectionModel> FilteredConnectionList
        {
            get => _filteredConnectionList;
            set => SetProperty(ref _filteredConnectionList, value);
        }

        private int _selectedProcessId = -1;

        private bool _isMonitoring = false;
        public bool IsMonitoring
        {
            get => _isMonitoring;
            set
            {
                SetProperty(ref _isMonitoring, value);
            }
        }

        public void FilterConnectionsByPid(int processId)
        {
            UpdateFilteredConnectionListByPid(processId, _internalConnectionList);
        }

        private void StartMonitoringCommandExecute()
        {
            IsMonitoring = true;
            DoGetData();
        }

        private void StopMonitoringCommandExecute()
        {
            IsMonitoring = false;
            _cancellationSource.Cancel();
        }

        private void ResetCancellationTokenSource()
        {
            _cancellationSource = new CancellationTokenSource();
        }

        private void DoGetData()
        {
            if (!IsMonitoring)
            {
                return;
            }

            Task.Run(() => {

                var connectionList = new List<ProcessConnectionModel>();

                while (!_cancellationToken.IsCancellationRequested) 
                {
                    try
                    {
                        IPHlpAPI32Wrapper.FillConnections(connectionList);

                        InvokeOnMainThreadSync(() =>
                        {
                            SetListsData(connectionList);
                        });

                        Thread.Sleep(100); // get data each 100 ms
                    }
                    catch (Exception ex)
                    {
                        InvokeOnMainThreadSync(() =>
                        {
                            IsMonitoring = false;
                        });

                        System.Windows.MessageBox.Show($"Failed to Get connections data {ex.Message}",
                            "NetConnectWatcher",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Error);
                    }
                }

                _cancellationToken.ThrowIfCancellationRequested();

            }, _cancellationToken)
            .ContinueWith(t =>
            {
                t.Exception?.Handle(e => true);
                ResetCancellationTokenSource();

            }, TaskContinuationOptions.OnlyOnCanceled);

        }

        private void SetListsData(List<ProcessConnectionModel> list)
        {
            UpdateCollection(_internalConnectionList, list);

            UpdateFilteredConnectionListByPid(_selectedProcessId, list);

            var processList = new Dictionary<int, ProcessConnectionModel>();

            foreach (var con in list)
            {
                if (!processList.ContainsKey(con.ProcessId))
                {
                    processList.Add(con.ProcessId, con);
                }
            }

            UpdateObservableCollection(ProcessList, processList.Values.ToList());
        }

        private void UpdateFilteredConnectionListByPid(int processId, List<ProcessConnectionModel> list)
        {
            if (processId != -1) // filtering
            {
                var filteredList = list.FindAll(x => x.ProcessId == processId);

                UpdateObservableCollection(FilteredConnectionList, filteredList);
            }
            else
            {
                UpdateObservableCollection(FilteredConnectionList, list);
            }

            _selectedProcessId = processId;
        }

        private void UpdateObservableCollection(ObservableCollection<ProcessConnectionModel> observableCollection, List<ProcessConnectionModel> list)
        {
            var processListCount = observableCollection.Count;

            for (int i = 0; i < list.Count; i++)
            {
                if (i < processListCount)
                {
                    observableCollection[i] = list[i].DeepCopy();
                }
                else
                {
                    observableCollection.Add(list[i].DeepCopy());
                }
            }

            if (observableCollection.Count > list.Count)
            {
                for (int i = observableCollection.Count - 1; i >= list.Count; i--)
                {
                    observableCollection.RemoveAt(i);
                }
            }
        }

        private void UpdateCollection(List<ProcessConnectionModel> collection, List<ProcessConnectionModel> list)
        {
            var processListCount = collection.Count;

            for (int i = 0; i < list.Count; i++)
            {
                if (i < processListCount)
                {
                    collection[i] = list[i].DeepCopy();
                }
                else
                {
                    collection.Add(list[i].DeepCopy());
                }
            }

            if (collection.Count > list.Count)
            {
                collection.RemoveRange(list.Count - 1, collection.Count - list.Count);
            }
        }

    }
}
