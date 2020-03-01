using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Input;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Windows;

namespace QM.Settings
{
    public class BasicSettingsViewModel : ObservableObject, IPageViewModel
    {
        #region Commands

        public ICommand BackCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ChangeServerContentFolderCommand { get; set; }
        public ICommand ChangeTestStorageFolderCommand { get; set; }
        public ICommand ChangeLogFolderCommand { get; set; }
        public ICommand ChangeDefaultSaveFolderCommand { get; set; }
        public ICommand RemovePrefixCommand { get; set; }
        public ICommand AddNewPrefixCommand { get; set; }

        #endregion

        #region Properties 

        public string Name => "Basic settings page";
        public string ServerContentFolderPath { get => _serverContentFolderPath; set { _serverContentFolderPath = value; _unsavedChanges = true; } }
        public string TestStorageFolderPath { get => _testStorageFolderPath; set { _testStorageFolderPath = value; _unsavedChanges = true; } }
        public string LogFolderPath { get => _logFolderPath; set { _logFolderPath = value; _unsavedChanges = true; } }
        public string DefaultSaveFolderPath { get => _defaultSaveFolderPath; set { _defaultSaveFolderPath = value; _unsavedChanges = true; } }
        public string NewPrefix { get => _newPrefix; set { _newPrefix = value; _unsavedChanges = true; } }
        public ObservableCollection<string> Prefixes { get => _prefixes; set { _prefixes = value; _unsavedChanges = true; } }
        public bool SaveAsCSV { get => _saveAsCSV; set { _saveAsCSV = value; _unsavedChanges = true; } }
        public bool SaveAsXLSX { get => _saveAsXLSX; set { _saveAsXLSX = value; _unsavedChanges = true; } }
        public bool AutoSave { get => _autoSave; set { _autoSave = value; _unsavedChanges = true; } }
        public bool FairPlay
        {
            get => !_ignoreCheating;
            set
            {
                _ignoreCheating = !value;
                _unsavedChanges = true;
                if (value == true)
                {
                    _ignoreDuplicates = false;
                    OnPropertyChanged("IgnoreDuplicates");
                }
            }
        }
        public bool IgnoreDuplicates
        {
            get => _ignoreDuplicates;
            set
            {
                _ignoreDuplicates = value;
                _unsavedChanges = true;
                if (value == true)
                {
                    _ignoreCheating = true;
                    OnPropertyChanged("FairPlay");
                }
            }
        }
        public bool RealTimeResult { get => !_hideResult; set { _hideResult = !value; _unsavedChanges = true; } }
        public bool MixQuestions { get => _mixQuestions; set { _mixQuestions = value; _unsavedChanges = true; } }
        public bool MixAnswers { get => _mixAnswers; set { _mixAnswers = value; _unsavedChanges = true; } }
        public int GroupsLimit { get => _groupsLimit; set { _groupsLimit = value; _unsavedChanges = true; } }
        public NetworkInterfaceType NetworkInterfaceType { get => _networkInterfaceType; set { _networkInterfaceType = value; _unsavedChanges = true; } }

        #endregion

        #region Events

        public event EventHandler<IPageViewModel> ChangePageEvent;

        #endregion

        #region Constructors

        public BasicSettingsViewModel(EventHandler<IPageViewModel> changePageEvent)
        {
            ChangePageEvent = changePageEvent;
            BackCommand = new RelayCommand(ExitToMainMenu);
            SaveCommand = new RelayCommand((object obj) => { SaveSettings(); ShowMainMenuPage(); });
            ChangeDefaultSaveFolderCommand = new RelayCommand(ChangeDefaultSaveFolder);
            ChangeTestStorageFolderCommand = new RelayCommand(ChangeTestStorageFolder);
            ChangeServerContentFolderCommand = new RelayCommand(ChangeServerContentFolder);
            ChangeLogFolderCommand = new RelayCommand(ChangeLogFolder);
            RemovePrefixCommand = new RelayCommand(RemovePrefix);
            AddNewPrefixCommand = new RelayCommand(AddNewPrefix,CanAddNewPrefix);

            ApplicationSettings applicationSettings = ServiceProvider.GetApplicationSettings;
            _serverContentFolderPath = applicationSettings.ServerContentFolderPath;
            _testStorageFolderPath = applicationSettings.TestStorageFolderPath;
            _logFolderPath = applicationSettings.LogFolderPath;
            _defaultSaveFolderPath = applicationSettings.DefaultSaveFolderPath;
            _prefixes = new ObservableCollection<string>(applicationSettings.Prefixes.ToList());
            _saveAsCSV = applicationSettings.SaveAsCSV;
            _saveAsXLSX = applicationSettings.SaveAsXLSL;
            _autoSave = applicationSettings.AutoSave;
            _ignoreCheating = applicationSettings.IgnoreCheating;
            _ignoreDuplicates = applicationSettings.IgnoreDuplicate;
            _hideResult = applicationSettings.HideResult;
            _groupsLimit = applicationSettings.GroupsLimit;
            _networkInterfaceType = applicationSettings.NetworkInterfaceType;
            _mixAnswers = applicationSettings.MixAnswers;
            _mixQuestions = applicationSettings.MixQuestions;

            _unsavedChanges = false;
        }

        #endregion

        #region Fields

        private string _serverContentFolderPath;
        private string _testStorageFolderPath;
        private string _logFolderPath;
        private string _defaultSaveFolderPath;
        private ObservableCollection<string> _prefixes;
        private bool _saveAsCSV;
        private bool _saveAsXLSX;
        private bool _autoSave;
        private bool _ignoreCheating;
        private bool _ignoreDuplicates;
        private bool _hideResult;
        private int _groupsLimit;
        private NetworkInterfaceType _networkInterfaceType;
        private string _newPrefix;
        private bool _unsavedChanges;
        private bool _mixQuestions;
        private bool _mixAnswers;

        #endregion

        #region Methods

        private void ShowMainMenuPage()
        {
            ChangePageEvent.Invoke(this, new MainMenu.MainMenuViewModel(ChangePageEvent));
        }

        private void SaveSettings()
        {
            if (_unsavedChanges)
            {
                ApplicationSettings applicationSettings = new ApplicationSettings
                {
                    AutoSave = _autoSave,
                    DefaultSaveFolderPath = _defaultSaveFolderPath,
                    GroupsLimit = _groupsLimit,
                    HideResult = _hideResult,
                    IgnoreCheating = _ignoreCheating,
                    IgnoreDuplicate = _ignoreDuplicates,
                    LogFolderPath = _logFolderPath,
                    NetworkInterfaceType = _networkInterfaceType,
                    Prefixes = _prefixes.ToArray(),
                    SaveAsCSV = _saveAsCSV,
                    SaveAsXLSL = _saveAsXLSX,
                    ServerContentFolderPath = _serverContentFolderPath,
                    TestStorageFolderPath = _testStorageFolderPath,
                    MixQuestions = _mixQuestions,
                    MixAnswers = _mixAnswers
                };

                ServiceProvider.GetApplicationSettings = applicationSettings;
                ServiceProvider.GetApplicationSettings.SaveSettings();
            }
        }

        private void ExitToMainMenu(object obj)
        {
            if (_unsavedChanges)
            {
                switch (System.Windows.MessageBox.Show("Are you sure you wish to discard these changes?", "Unsaved changes.", MessageBoxButton.YesNo, MessageBoxImage.Warning))
                {
                    case MessageBoxResult.Yes:
                        ShowMainMenuPage();
                        break;
                    case MessageBoxResult.No:
                        return;
                    default:
                        break;
                }
            }

            ShowMainMenuPage();
        }

        private void ChangeServerContentFolder(object obj)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _unsavedChanges = true;
                    ServerContentFolderPath = dialog.SelectedPath;
                }
            }
        }

        private void ChangeDefaultSaveFolder(object obj)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _unsavedChanges = true;
                    DefaultSaveFolderPath = dialog.SelectedPath;
                }
            }
        }

        private void ChangeLogFolder(object obj)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _unsavedChanges = true;
                    LogFolderPath = dialog.SelectedPath;
                }
            }
        }

        private void ChangeTestStorageFolder(object obj)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    TestStorageFolderPath = dialog.SelectedPath;
                    _unsavedChanges = true;
                }
            }
        }

        private void RemovePrefix(object obj)
        {
            _unsavedChanges = true;
            Prefixes.Remove((string)obj);
        }

        private void AddNewPrefix(object obj)
        {
            _unsavedChanges = true;
            Prefixes.Add(NewPrefix);
        }

        private bool CanAddNewPrefix(object obj)
        {
            if (_newPrefix is null)
            {
                return false;
            }

            if ((_newPrefix.StartsWith(@"http://") || _newPrefix.StartsWith(@"https://")) && _newPrefix.EndsWith(@"/"))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}