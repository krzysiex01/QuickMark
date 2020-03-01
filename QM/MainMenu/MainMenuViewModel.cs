using System;
using System.Windows.Input;

namespace QM.MainMenu
{
    public class MainMenuViewModel : ObservableObject, IPageViewModel
    {
        #region Properties 

        public string Name => "Main menu page";

        #endregion

        #region Events

        public event EventHandler<IPageViewModel> ChangePageEvent;

        #endregion

        #region Constructors

        public MainMenuViewModel(EventHandler<IPageViewModel> changePageViewModel)
        {
            ChangePageEvent = changePageViewModel;

            CreateTestCommand = new RelayCommand(ShowTypeSelectorPage);
            LoadTestCommand = new RelayCommand(ShowTestLoaderPage);
            SettingsCommand = new RelayCommand(ShowBasicSettingsPage);
        }

        #endregion

        #region Commands

        public ICommand CreateTestCommand { get; set; }
        public ICommand LoadTestCommand { get; set; }
        public ICommand SettingsCommand { get; set; }


        #endregion

        #region Methods

        private void ShowTestLoaderPage(object obj)
        {
            ChangePageEvent.Invoke(this, new TestMaker.TestLoaderViewModel(ChangePageEvent));
        }

        private void ShowTypeSelectorPage(object obj)
        {
            ChangePageEvent.Invoke(this, new TestCreator.TypeSelectorViewModel(ChangePageEvent));
        }

        private void ShowBasicSettingsPage(object obj)
        {
            ChangePageEvent.Invoke(this, new Settings.BasicSettingsViewModel(ChangePageEvent));
        }

        #endregion
    }
}