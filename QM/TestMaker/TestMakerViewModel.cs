using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace QM.TestMaker
{
    public class TestMakerViewModel : ObservableObject, IPageViewModel
    {
        #region Events

        public event EventHandler<IPageViewModel> ChangePageEvent;

        #endregion

        #region Fields

        private Server Server { get; set; }
        private TestManager Test { get; set; }
        private readonly IServiceProvider _serviceProvider;
        private Color _serverStateColorFrom;
        private Color _serverStateColorTo;
        private ObservableCollection<Record> _records;
        private ListSortDirection _sortDirection;
        private CollectionViewSource _recordsViewSource;
        private string _serverStateInfo;
        private string _sortColumn;
        private bool _isServerStarted;
        private bool _isServerPaused;

        #endregion

        #region Commands

        public ICommand StartCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand HelpCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand SortColumnCommand { get; set; }

        #endregion

        #region Properties

        public string Name => "TestMaker page";
        public ObservableCollection<Record> Records
        {
            get
            {
                return _records;
            }
            set
            {
                _records = value;
                _recordsViewSource = new CollectionViewSource
                {
                    Source = _records
                };
            }
        }
        public ListCollectionView RecordsView
        {
            get
            {
                return (ListCollectionView)_recordsViewSource.View;
            }
        }
        public string ServerStateInfo { get => _serverStateInfo; set { _serverStateInfo = value; OnPropertyChanged("ServerStateInfo"); } }
        public string TestName { get; set; }
        public Color ServerStateColorFrom { get => _serverStateColorFrom; set { _serverStateColorFrom = value; OnPropertyChanged("ServerStateColorFrom"); } }
        public Color ServerStateColorTo { get => _serverStateColorTo; set { _serverStateColorTo = value; OnPropertyChanged("ServerStateColorTo"); } }
        public bool IsServerStarted { get => _isServerStarted; set { _isServerStarted = value; OnPropertyChanged("IsServerStarted"); } }
        public bool IsServerPaused { get => _isServerPaused; set { _isServerPaused = value; OnPropertyChanged("IsServerPaused"); } }

        #endregion

        #region Constructors

        public TestMakerViewModel(EventHandler<IPageViewModel> changePageEvent, TestManager test, IServiceProvider serviceProvider = null): this(serviceProvider)
        {
            ChangePageEvent = changePageEvent;
            Server = new Server(test, _serviceProvider.GetApplicationSettings.Prefixes, _serviceProvider.GetApplicationSettings.ServerContentFolderPath);
            Test = test;
            Records = Test.records;
            TestName = test.TestId;
            _serverStateColorFrom = Colors.HotPink;
            _serverStateColorTo = Colors.DarkRed;
            _serverStateInfo = "Ready. Press START to run the server.";
            _isServerStarted = false;
            _isServerPaused = true;
            StartCommand = new RelayCommand(StartPauseServer);
            SaveCommand = new RelayCommand(SaveRecords);
            HelpCommand = new RelayCommand(ShowHelpWindow);
            ResetCommand = new RelayCommand(Reset);
            ExitCommand = new RelayCommand(Exit);
            SortColumnCommand = new RelayCommand(Sort);
        }

        public TestMakerViewModel(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                _serviceProvider = new ServiceProviderWrapper();
            }
            else
            {
                _serviceProvider = serviceProvider;
            }
        }

        #endregion

        #region Methods

        private void StartPauseServer(object obj)
        {
            if (IsServerStarted)
            {
                Server.Stop();
                ServerStateInfo = "Server paused. Press START to resume.";
                ServerStateColorFrom = Colors.LawnGreen;
                ServerStateColorTo = Colors.DarkGreen;
                IsServerStarted = false;
                IsServerPaused = true;
            }
            else
            {
                Server.StartAsync();
                ServerStateInfo = "Server is running...";
                ServerStateColorFrom = Colors.LawnGreen;
                ServerStateColorTo = Colors.DarkGreen;
                IsServerStarted = true;
                IsServerPaused = false;
            }
        }

        private void SaveRecords(object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel file (*.xlsx)|*.xlsx|CSV file (*.csv)|*.csv",
                Title = "Save records as ..."
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                Test.SaveRecordsAs(saveFileDialog.FileName);
                ServerStateInfo = "Saved succesfully.";
            }
            else
            {
                ServerStateInfo = "You have unsaved records.";
            }
        }

        private void ShowHelpWindow(object obj)
        {
            Help.HelpWindow window = new Help.HelpWindow();

            var viewModel = new Help.HelpWindowViewModel(Server.GetLocalIPv4());

            window.DataContext = viewModel;
            window.Show();
        }

        private void Reset(object obj)
        {
            Server.Stop();
            Records.Clear();
            IsServerPaused = true;
            IsServerStarted = false;
            ServerStateInfo = "Records cleared. Press START to run the server.";
        }

        private void Exit(object obj)
        {
            Server.Stop();
            Server.Close();
            Test.AutoSaveRecords();
            ChangePageEvent.Invoke(this, new MainMenu.MainMenuViewModel(ChangePageEvent));
        }

        private void Sort(object obj)
        {
            string column = obj as string;
            if (_sortColumn == column)
            {
                _sortDirection = _sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            else
            {
                _sortColumn = column;
                _sortDirection = ListSortDirection.Ascending;
            }

            _recordsViewSource.SortDescriptions.Clear();
            _recordsViewSource.SortDescriptions.Add(new SortDescription(_sortColumn, _sortDirection));
        }

        #endregion
    }
}
