using System;
using System.Windows.Input;

namespace QM
{
    public class ApplicationViewModel : ObservableObject
    {
        #region Fields

        private ICommand _changePageCommand;
        private ICommand _closeApplicationCommand;
        private IPageViewModel _currentPageViewModel;

        #endregion

        #region Constructors

        public ApplicationViewModel()
        {
            // Subscribe to event
            ChangePageEvent += OnChangePageViewModel;

            // Assign RelayCommands to ICommands fields
            _closeApplicationCommand = new RelayCommand((obj) => { CloseApplication((ICloseable)obj); });

            // Set starting page
            CurrentPageViewModel = new MainMenu.MainMenuViewModel(ChangePageEvent);
        }

        #endregion

        #region Commands

        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(
                        p => ChangeViewModel((IPageViewModel)p),
                        p => p is IPageViewModel);
                }

                return _changePageCommand;
            }
        }
        public ICommand CloseApplicationCommand { get => _closeApplicationCommand; set => _closeApplicationCommand = value; }

        #endregion

        #region Properties

        /// <summary>
        /// Page to be displayed in main window.
        /// </summary>
        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }
        
        #endregion

        #region Events
        
        /// <summary>
        /// Invoke to change page to be displayed in main window.
        /// </summary>
        public event EventHandler<IPageViewModel> ChangePageEvent;

        #endregion

        #region Methods

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            CurrentPageViewModel = viewModel;
        }

        private void CloseApplication(ICloseable window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        private void OnChangePageViewModel(object sender, IPageViewModel pageViewModel)
        {
            ChangeViewModel(pageViewModel);
        }

        #endregion
    }

    public interface IPageViewModel
    {
        string Name { get; }

        event EventHandler<IPageViewModel> ChangePageEvent;
    }
}
