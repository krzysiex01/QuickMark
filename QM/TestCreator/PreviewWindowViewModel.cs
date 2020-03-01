using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Printing;
using System.IO;
using System.Windows.Xps;

namespace QM.TestCreator
{
    public class PreviewWindowViewModel
    {
        #region Properties

        public List<FlowDocument> Documents { get => _documents; set => _documents = value; }

        #endregion

        #region Constructors

        public PreviewWindowViewModel(List<TestGroupViewModel> testGroups, bool autogroups)
        {
            PrintCommand = new RelayCommand(Print);
            PrintAllCommand = new RelayCommand(PrintAll);
            ConvertCommand = new RelayCommand(Convert);

            _documents = new List<FlowDocument>();
            _answers = new string[testGroups.Count][][];
            _correctAnswers = new string[testGroups.Count][];
            _questions = new string[testGroups.Count][];
            _numberOfGroups = testGroups.Count;
            int groupCount = 0;
            foreach (var testGroup in testGroups)
            {
                _answers[groupCount] = testGroup.TestQuestionList.Select((TestCreator.TestQuestionViewModel tq) => { return tq.Answers.ToArray(); }).ToArray();
                _correctAnswers[groupCount] = testGroup.TestQuestionList.Select((TestCreator.TestQuestionViewModel tq) => { return tq.CorrectAnswer; }).ToArray();
                _questions[groupCount] = testGroup.TestQuestionList.Select((TestCreator.TestQuestionViewModel tq) => { return tq.Question; }).ToArray();
                groupCount++;
            }

            if (autogroups)
            {
                AutoGroupsHelper autoGroupsHelper = new AutoGroupsHelper();
                _numberOfGroups = autoGroupsHelper.ShuffleTest(_answers[0], _correctAnswers[0], _questions[0],
                    out string[][][] answers, out string[][] correctAnswers, out string[][] questions, ServiceProvider.GetApplicationSettings.MixAnswers, ServiceProvider.GetApplicationSettings.MixQuestions);
                _answers = answers;
                _questions = questions;
                _correctAnswers = correctAnswers;
            }

            groupCount = 1;
            for (int i = 0; i < _questions.Length; i++)
            {
                FlowDocument document = new FlowDocument();
                document.IsOptimalParagraphEnabled = true;
                document.IsHyphenationEnabled = true;
                Paragraph firstParagraph = new Paragraph(new Run($"Group {groupCount}"));
                firstParagraph.TextAlignment = TextAlignment.Right;
                document.Blocks.Add(firstParagraph);
                document.Tag = groupCount++;

                int questionCount = 1;
                for (int j = 0; j < _questions[i].Length; j++)
                {
                    Paragraph p = new Paragraph(new Run(questionCount++ + ". " + _questions[i][j]));
                    p.FontSize = 12;
                    p.FontWeight = FontWeights.Bold;
                    p.Margin = new Thickness(2);
                    document.Blocks.Add(p);

                    for (int k = 0; k < _answers[i][j].Length; k++)
                    {
                        Paragraph pp = new Paragraph(new Run((char)((int)'a' + k) + ") " + _answers[i][j][k]));
                        pp.FontSize = 11;
                        pp.Margin = new Thickness(0);
                        pp.FontWeight = FontWeights.Normal;
                        document.Blocks.Add(pp);
                    }
                }

                _documents.Add(document);
            }
        }

        #endregion

        #region Fields

        private List<FlowDocument> _documents;
        private string[][][] _answers;
        private string[][] _correctAnswers;
        private string[][] _questions;
        private int _numberOfGroups;

        #endregion

        #region Commands

        public ICommand PrintCommand { get; set; }
        public ICommand PrintAllCommand { get; set; }
        public ICommand ConvertCommand { get; set; }

        #endregion

        #region Methods

        private void Print(object obj)
        {
            if (obj is int selectedIndex)
            {
                // Create copy of selected FlowDocument
                MemoryStream s = new MemoryStream();
                TextRange source = new TextRange(_documents[selectedIndex].ContentStart, _documents[selectedIndex].ContentEnd);
                source.Save(s, System.Windows.DataFormats.Xaml);
                FlowDocument copy = new FlowDocument();
                TextRange dest = new TextRange(copy.ContentStart, copy.ContentEnd);
                dest.Load(s, System.Windows.DataFormats.Xaml);

                // Create a XpsDocumentWriter object, implicitly opening a Windows common print dialog,
                // and allowing the user to select a printer.
                PrintDocumentImageableArea printDocumentImageableArea = null;
                XpsDocumentWriter xpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(ref printDocumentImageableArea);

                if (xpsDocumentWriter != null && printDocumentImageableArea != null)
                {
                    DocumentPaginator documentPaginator = ((IDocumentPaginatorSource)copy).DocumentPaginator;

                    // Change the PageSize and PagePadding for the document to match the CanvasSize for the printer device.
                    documentPaginator.PageSize = new Size(printDocumentImageableArea.MediaSizeWidth, printDocumentImageableArea.MediaSizeHeight);
                    Thickness t = new Thickness(72);
                    // set PagePadding;
                    copy.PagePadding = new Thickness(
                                     Math.Max(printDocumentImageableArea.OriginWidth, t.Left),
                                       Math.Max(printDocumentImageableArea.OriginHeight, t.Top),
                                       Math.Max(printDocumentImageableArea.MediaSizeWidth - (printDocumentImageableArea.OriginWidth + printDocumentImageableArea.ExtentWidth), t.Right),
                                       Math.Max(printDocumentImageableArea.MediaSizeHeight - (printDocumentImageableArea.OriginHeight + printDocumentImageableArea.ExtentHeight), t.Bottom));

                    copy.ColumnWidth = double.PositiveInfinity;

                    // Send content to the printer.
                    xpsDocumentWriter.Write(documentPaginator);
                }
            }
        }

        private void PrintAll(object obj)
        {
            for (int i = 0; i < _numberOfGroups; i++)
            {
                Print(i);
            }
        }

        private void Convert(object obj)
        {
            TestSerializer testSerializer = new TestSerializer();
            testSerializer.SaveAndConvertTest(_answers,_correctAnswers,_questions,_numberOfGroups);
        }

        #endregion
    }
}
