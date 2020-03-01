using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Input;

namespace QM.TestMaker
{
    public class TestLoaderViewModel : ObservableObject, IPageViewModel
    {
        #region Properties

        public string Name => "Test loader page";
        public TestLoaderHelper TestLoaderHelper { get; set; }
        public TestDataPreview SelectedTestDataPreview { get; set; }

        #endregion

        #region Events

        public event EventHandler<IPageViewModel> ChangePageEvent;

        #endregion

        #region Commands

        public ICommand LoadTestCommand { get; set; }
        public ICommand BrowseCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        #endregion

        #region Constructors

        public TestLoaderViewModel(EventHandler<IPageViewModel> changePageEvent)
        {
            LoadTestCommand = new RelayCommand(LoadTest, CanLoadTest);
            BrowseCommand = new RelayCommand(Browse);
            BackCommand = new RelayCommand(ShowMainMenuPage);
            RefreshCommand = new RelayCommand(RefreshList);
            TestLoaderHelper = new TestLoaderHelper();
            TestLoaderHelper.RefreshPreviewsList();

            ChangePageEvent = changePageEvent;
        }

        #endregion

        #region Methods

        private void LoadTest(object obj)
        {
            TestManager test = new TestManager(ServiceProvider.GetApplicationSettings);
            if (test.LoadTestData(SelectedTestDataPreview.FullPath))
            {
                ChangePageEvent.Invoke(this, new TestMakerViewModel(ChangePageEvent, test));
            }
        }

        private bool CanLoadTest(object obj)
        {
            if (SelectedTestDataPreview is null)
            {
                return false;
            }
            return true;
        }

        private void Browse(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "QM test (*.qm)|*.qm",
                Title = "Select Qucik Mark test ..."
            };
            if (openFileDialog.ShowDialog() == true)
            {
                TestManager test = new TestManager(ServiceProvider.GetApplicationSettings);
                if (test.LoadTestData(openFileDialog.FileName))
                {
                    string destinationPath = Path.Combine(ServiceProvider.GetApplicationSettings.TestStorageFolderPath, Path.GetFileName(openFileDialog.FileName));

                    if (!File.Exists(destinationPath))//do backup copy of selected file
                    {
                        File.Copy(openFileDialog.FileName, destinationPath);
                    }
                    else if (openFileDialog.FileName == destinationPath)//the selected file is the backupfile stored in TestStorageFolderPath
                    {
                        File.SetLastAccessTime(destinationPath, DateTime.Now);
                    }
                    else//replace
                    {
                        File.Delete(destinationPath);
                        File.Copy(openFileDialog.FileName, destinationPath);
                        File.SetLastAccessTime(destinationPath, DateTime.Now);
                    }
                    ChangePageEvent.Invoke(this, new TestMakerViewModel(ChangePageEvent, test));
                }
            }
        }

        private void ShowMainMenuPage(object obj)
        {
            ChangePageEvent.Invoke(this, new MainMenu.MainMenuViewModel(ChangePageEvent));
        }

        private void RefreshList(object obj)
        {
            TestLoaderHelper.RefreshPreviewsList();
            OnPropertyChanged("TestLoaderHelper");
        }

        #endregion
    }
}
