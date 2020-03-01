using System.Windows.Input;

namespace QM.Help
{
    class HelpWindowViewModel
    {
        #region Constructors

        public HelpWindowViewModel(string ipaddress)
        {
            IPAddress = ipaddress;
            OpenUserGuide = new RelayCommand(OpenUserGuideFile);
        }

        #endregion

        #region Properties

        public string IPAddress { get; set; }

        #endregion

        #region Commands

        public ICommand OpenUserGuide { get; set; }

        #endregion

        #region Methods

        private void OpenUserGuideFile(object obj)
        {
            //TODO:USERGUIDE
        }

        #endregion
    }
}
