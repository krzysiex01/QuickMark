using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QM.TestCreator
{
    public class TestCreatorViewModel : ObservableObject, IPageViewModel
    {
        #region Properties

        public string Name => "Test creator page";
        public int NumberOfGroups { get => _numberOfGroups; set => _numberOfGroups = value; }
        public bool DisplayQuestions { get => _displayQuestions; set => _displayQuestions = value; }
        public bool AutoGroups { get => _autoGroups; set => _autoGroups = value; }
        public AnswerType AnswerType { get => _answerType; set => _answerType = value; }
        public List<TestGroupViewModel> TestGroups { get => _testGroups; set => _testGroups = value; }

        #endregion

        #region Commands

        public ICommand BackCommand { get; set; }
        public ICommand SaveTestCommand { get; set; }
        public ICommand PreviewCommand { get; set; }

        #endregion

        #region Events

        public event EventHandler<IPageViewModel> ChangePageEvent;

        #endregion

        #region Constructors

        public TestCreatorViewModel(EventHandler<IPageViewModel> changePageEvent, int numberOfGroups, bool displayQuestions, bool autoGroups, AnswerType answerType)
        {
            ChangePageEvent = changePageEvent;

            NumberOfGroups = numberOfGroups;
            AnswerType = answerType;
            AutoGroups = autoGroups;
            DisplayQuestions = displayQuestions;
            BackCommand = new RelayCommand(ShowTypeSelectorPage);
            SaveTestCommand = new RelayCommand(SaveTest, CanSaveTest);
            PreviewCommand = new RelayCommand(PreviewTest, CanSaveTest);

            _testGroups = new List<TestGroupViewModel>();

            if (AutoGroups)
            {
                TestGroupViewModel testGroup = new TestGroupViewModel(answerType, displayQuestions);
                testGroup.Header = "New set of questions";
                _testGroups.Add(testGroup);
            }
            else
            {
                for (int i = 0; i < NumberOfGroups; i++)
                {
                    TestGroupViewModel testGroup = new TestGroupViewModel(answerType, displayQuestions);
                    testGroup.Header = "Group " + (i + 1);
                    _testGroups.Add(testGroup);
                }
            }
        }

        public TestCreatorViewModel(EventHandler<IPageViewModel> changePageEvent, TestData testData)
        {
            CreateViewModels(testData);

            ChangePageEvent = changePageEvent;
            NumberOfGroups = testData.NumberOfGroups;
            AnswerType = AnswerType.Unspecified;
            AutoGroups = testData.AutoGroups;
            DisplayQuestions = testData.DigitalTest;
            BackCommand = new RelayCommand(ShowTypeSelectorPage);
            SaveTestCommand = new RelayCommand(SaveTest, CanSaveTest);
            PreviewCommand = new RelayCommand(PreviewTest, CanSaveTest);
        }

        #endregion

        #region Fields

        private List<TestGroupViewModel> _testGroups;
        private int _numberOfGroups;
        private bool _displayQuestions;
        private bool _autoGroups;
        private AnswerType _answerType;

        #endregion

        #region Methods

        private void ShowTypeSelectorPage(object obj)
        {
            ChangePageEvent.Invoke(this, new TypeSelectorViewModel(ChangePageEvent));
        }

        private void SaveTest(object obj)
        {
            TestSerializer testSerializer = new TestSerializer();
            if (testSerializer.SaveTest(_testGroups, _autoGroups, _displayQuestions, _numberOfGroups))
            {
                ChangePageEvent.Invoke(this, new MainMenu.MainMenuViewModel(ChangePageEvent));
            }
        }

        private bool CanSaveTest(object obj)
        {
            for (int i = 0; i < TestGroups.Count; i++)
            {
                if (TestGroups[i].TestQuestionList.Count != TestGroups[0].TestQuestionList.Count)
                        return false;

                if (TestGroups[i].TestQuestionList.Count == 0)
                    return false;

                for (int j = 0; j < TestGroups[i].TestQuestionList.Count; j++)
                {
                    if (TestGroups[i].TestQuestionList[j].CorrectAnswer == null 
                        || TestGroups[i].TestQuestionList[j].Answers.IndexOf(TestGroups[i].TestQuestionList[j].CorrectAnswer) == -1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void PreviewTest(object obj)
        {
            PreviewWindow window = new PreviewWindow();

            PreviewWindowViewModel previewWindowViewModel = new PreviewWindowViewModel(TestGroups,_autoGroups);
            window.DataContext = previewWindowViewModel;

            window.Show();
        }

        private void CreateViewModels(TestData testData)
        {
            List<TestGroupViewModel> testGroupViewModels = new List<TestGroupViewModel>();

            if (testData.AutoGroups)
            {
                TestGroupViewModel testGroup = new TestGroupViewModel(testData.DigitalTest, testData.Questions[0], testData.Answers[0], testData.CorrectAnswers[0]);
                testGroup.Header = "New set of questions";
                testGroupViewModels.Add(testGroup);
            }
            else
            {
                for (int i = 0; i < testData.NumberOfGroups; i++)
                {
                    TestGroupViewModel testGroup = new TestGroupViewModel(testData.DigitalTest, testData.Questions[i], testData.Answers[i], testData.CorrectAnswers[i]);
                    testGroup.Header = "Group " + (i + 1);
                    testGroupViewModels.Add(testGroup);
                }
            }

            _testGroups = testGroupViewModels;
        }

        #endregion
    }
    /// <summary>
    /// View-Model for TabControl item.
    /// </summary>
    public class TestGroupViewModel
    {
        public string Header { get; set; }
        public ObservableCollection<TestQuestionViewModel> TestQuestionList { get; set; }
        public ICommand RemoveQuestionCommand { get; set; }
        public ICommand AddQuestionCommand { get; set; }
        private bool CanEditQuestion { get; set; }
        private AnswerType AnswersType { get; set; }

        public TestGroupViewModel(AnswerType answerType, bool canEditQuestion)
        {
            TestQuestionList = new ObservableCollection<TestQuestionViewModel>();
            AddQuestionCommand = new RelayCommand(AddQuestion);
            RemoveQuestionCommand = new RelayCommand(RemoveQuestion);
            AnswersType = answerType;
            CanEditQuestion = canEditQuestion;
        }

        public TestGroupViewModel(bool canEditQuestion, string[] questions, string[][] answers, string[] correctAnswers)
        {
            TestQuestionList = new ObservableCollection<TestQuestionViewModel>();
            AddQuestionCommand = new RelayCommand(AddQuestion);
            RemoveQuestionCommand = new RelayCommand(RemoveQuestion);
            AnswersType = AnswerType.Unspecified;
            CanEditQuestion = canEditQuestion;

            for (int i = 0; i < questions.Length; i++)
            {
                TestQuestionViewModel testQuestionViewModel = new TestQuestionViewModel(canEditQuestion,questions[i],answers[i],correctAnswers[i]);
                testQuestionViewModel.Number = i + 1;
                TestQuestionList.Add(testQuestionViewModel);
            }
        }

        public TestGroupViewModel()
        {

        }

        private void AddQuestion(object obj)
        {
            TestQuestionViewModel testQuestionViewModel = new TestQuestionViewModel(AnswersType, CanEditQuestion);
            testQuestionViewModel.Number = TestQuestionList.Count + 1;
            if (AnswersType == AnswerType.ABC)
            {
                if (TestQuestionList.Count == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        testQuestionViewModel.AddAnswerCommand.Execute(null);
                    }
                }
                else
                {
                    for (int i = 0; i < TestQuestionList[TestQuestionList.Count - 1].Answers.Count; i++)
                    {
                        testQuestionViewModel.AddAnswerCommand.Execute(null);
                    }

                }
            }
            TestQuestionList.Add(testQuestionViewModel);
        }

        private void RemoveQuestion(object obj)
        {
            if (obj is TestQuestionViewModel testQuestionViewModel)
            {
                int number = testQuestionViewModel.Number;
                TestQuestionList.Remove(testQuestionViewModel);
                foreach (var item in TestQuestionList.Skip(number - 1))
                {
                    item.Number -= 1;
                    item.RaisePropertyChanged("Number");
                }
            }
        }
    }
    /// <summary>
    /// View-Model for ListView item.
    /// </summary>
    public class TestQuestionViewModel : ObservableObject
    {
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }
        public string NewAnswer { get; set; }
        public int Number { get; set; }
        public ObservableCollection<string> Answers { get; set; }
        public AnswerType AnswersType { get; set; }
        public Visibility IsNewAnswerTextBoxVisible { get; set; }
        public Visibility IsQuestionTextBoxVisible { get; set; }
        public Visibility IsAddNewAnswerButtonVisible { get; set; }

        public ICommand RemoveAnswerCommand { get; set; }
        public ICommand AddAnswerCommand { get; set; }

        public TestQuestionViewModel(AnswerType answerType, bool canEditQuestion)
        {
            Answers = new ObservableCollection<string>();
            RemoveAnswerCommand = new RelayCommand(RemoveAnswer);
            AddAnswerCommand = new RelayCommand(AddAnswer, CanAddAnswer);
            IsQuestionTextBoxVisible = canEditQuestion ? Visibility.Visible : Visibility.Collapsed;
            AnswersType = answerType;
            NewAnswer = "";
            Question = "";

            switch (answerType)
            {
                case AnswerType.TrueFalse:
                    Answers.Add("True");
                    Answers.Add("False");
                    IsNewAnswerTextBoxVisible = Visibility.Collapsed;
                    IsAddNewAnswerButtonVisible = Visibility.Collapsed;
                    break;
                case AnswerType.ABC:
                    NewAnswer = Alphabet.A.ToString();
                    IsNewAnswerTextBoxVisible = Visibility.Collapsed;
                    IsAddNewAnswerButtonVisible = Visibility.Visible;
                    break;
                case AnswerType.Unspecified:
                    IsNewAnswerTextBoxVisible = Visibility.Visible;
                    IsAddNewAnswerButtonVisible = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        public TestQuestionViewModel(bool canEditQuestion, string question, string[] answers, string correctAnswers): this(AnswerType.Unspecified, canEditQuestion)
        {
            Question = question;
            CorrectAnswer = correctAnswers;
            Answers = new ObservableCollection<string>(answers);
        }

        private void RemoveAnswer(object obj)
        {
            if (AnswersType == AnswerType.ABC)
            {
                Answers.RemoveAt(Answers.Count - 1);
            }
            else if (AnswersType == AnswerType.Unspecified)
            {
                Answers.Remove((string)obj);
            }
        }

        private void AddAnswer(object obj)
        {
            if (AnswersType == AnswerType.ABC)
            {
                NewAnswer = ((Alphabet)(Answers.Count)).ToString();
                Answers.Add(NewAnswer);
            }
            else
            {
                Answers.Add(NewAnswer);
                NewAnswer = "";
            }
            OnPropertyChanged("NewAnswer");
        }

        private bool CanAddAnswer(object obj)
        {
            if (AnswersType == AnswerType.ABC && Answers.Count >= 26)
            {
                return false;
            }

            return true;
        }
    }
}
