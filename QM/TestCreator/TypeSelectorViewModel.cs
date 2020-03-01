using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace QM.TestCreator
{
    public class TypeSelectorViewModel : ObservableObject, IPageViewModel
    {
        #region Properties

        public string Name => "Type selector page";
        public int NumberOfGroups { get => _numberOfGroups; set => _numberOfGroups = value; }
        public bool AutoGroups { get => _autoGroups; set => _autoGroups = value; }
        public string SelectedTestType
        {
            get => _selectedTestType;
            set
            {
                if (value != _selectedTestType)
                {
                    _selectedTestType = value;
                    OnPropertyChanged("SelectedTestType");
                }
            }
        }
        public int SelectedAnswerIndex { get => _selectedAnswerIndex; set { _selectedAnswerIndex = value; OnPropertyChanged("SelectedAnswerIndex"); } }

        #endregion

        #region Commands

        public ICommand BackCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand SetTestTypeCommand { get; set; }
        public ICommand OpenAndEditCommand { get; set; }

        #endregion

        #region Events

        public event EventHandler<IPageViewModel> ChangePageEvent;

        #endregion

        #region Constructors

        public TypeSelectorViewModel(EventHandler<IPageViewModel> changePageEvent)
        {
            ChangePageEvent = changePageEvent;

            NextCommand = new RelayCommand(ShowTestCreatorPage);
            BackCommand = new RelayCommand(ShowMainMenuPage);
            SetTestTypeCommand = new RelayCommand(SetTestType);
            OpenAndEditCommand = new RelayCommand(OpenAndEdit);

            _numberOfGroups = 1;
        }

        #endregion

        #region Fields

        private int _numberOfGroups;
        private bool _displayQuestions;
        private bool _autoGroups;
        private string _selectedTestType;
        private int _selectedAnswerIndex;

        #endregion

        #region Methods

        private void ShowTestCreatorPage(object obj)
        {
            AnswerType answerType = (AnswerType)SelectedAnswerIndex;

            ChangePageEvent.Invoke(this, new TestCreatorViewModel(ChangePageEvent,_numberOfGroups,_displayQuestions,_autoGroups,answerType));
        }

        private void ShowMainMenuPage(object obj)
        {
            ChangePageEvent.Invoke(this, new MainMenu.MainMenuViewModel(ChangePageEvent));
        }

        private void SetTestType(object obj)
        {
            if (obj is string s)
            {
                if (s == "Digital test")
                {
                    SelectedTestType = "DigitalTestDataTemplate";
                    SelectedAnswerIndex = 0;
                    _displayQuestions = true;
                }
                else if (s == "Exam test")
                {
                    SelectedTestType = "ExamTestDataTemplate";
                    SelectedAnswerIndex = 0;
                    AutoGroups = false;
                    _displayQuestions = false;
                }
            }
        }

        private void OpenAndEdit(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "QM test (*.qm)|*.qm",
                Title = "Select Qucik Mark test ..."
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TestSerializer testSerializer = new TestSerializer();
                TestData testData = testSerializer.LoadTest(openFileDialog.FileName);
                if (testData != null)
                {
                    ChangePageEvent.Invoke(this, new TestCreatorViewModel(ChangePageEvent, testData));
                }
            }
        }
        
        #endregion
    }
}
