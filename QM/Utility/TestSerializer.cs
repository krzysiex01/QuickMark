using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QM
{
    public class TestSerializer
    {
        public bool SaveTest(List<TestCreator.TestGroupViewModel> testGroups, bool autoGroups, bool displayQuestions,int numberOfGroups)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Quick Mark test (*.qm)|*.qm",
                Title = "Save test as ..."
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                TestData testData = new TestData
                {
                    AutoGroups = autoGroups,
                    DigitalTest = displayQuestions,
                    TestId = Path.GetFileNameWithoutExtension(saveFileDialog.FileName),
                    NumberOfGroups = autoGroups ? 1 : numberOfGroups,
                    Answers = new string[autoGroups ? 1 : numberOfGroups][][],
                    CorrectAnswers = new string[autoGroups ? 1 : numberOfGroups][],
                    Questions = new string[autoGroups ? 1 : numberOfGroups][]
                };

                for (int i = 0; i < testData.NumberOfGroups; i++)
                {
                    testData.Questions[i] = new string[testGroups[i].TestQuestionList.Count];
                    testData.Answers[i] = new string[testGroups[i].TestQuestionList.Count][];
                    testData.CorrectAnswers[i] = new string[testGroups[i].TestQuestionList.Count];
                    for (int j = 0; j < testGroups[i].TestQuestionList.Count; j++)
                    {
                        testData.Questions[i][j] = testGroups[i].TestQuestionList[j].Question;
                        testData.CorrectAnswers[i][j] = testGroups[i].TestQuestionList[j].CorrectAnswer;
                        testData.Answers[i][j] = new string[testGroups[i].TestQuestionList[j].Answers.Count];
                        for (int k = 0; k < testGroups[i].TestQuestionList[j].Answers.Count; k++)
                        {
                            testData.Answers[i][j][k] = testGroups[i].TestQuestionList[j].Answers[k];
                        }
                    }
                }

                FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(fs, testData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
                finally
                {
                    fs.Close();
                }

                return true;
            }

            return false;
        }

        public void SaveAndConvertTest(string[][][] answers, string[][] correctAnswers, string[][] questions, int numberOfGroups)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Quick Mark test (*.qm)|*.qm",
                Title = "Save test as ..."
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                TestData testData = new TestData
                {
                    AutoGroups = false,
                    DigitalTest = false,
                    TestId = Path.GetFileNameWithoutExtension(saveFileDialog.FileName),
                    NumberOfGroups = numberOfGroups,
                    Answers = new string[numberOfGroups][][],
                    CorrectAnswers = new string[numberOfGroups][],
                    Questions = new string[numberOfGroups][]
                };

                for (int i = 0; i < testData.NumberOfGroups; i++)
                {
                    testData.Questions[i] = new string[questions[i].Length];
                    testData.Answers[i] = new string[questions[i].Length][];
                    testData.CorrectAnswers[i] = new string[questions[i].Length];
                    for (int j = 0; j < questions[i].Length; j++)
                    {
                        testData.Questions[i][j] = "";
                        testData.CorrectAnswers[i][j] = ((char)((int)'A' + Array.IndexOf(answers[i][j],correctAnswers[i][j]))).ToString();
                        testData.Answers[i][j] = new string[answers[i][j].Length];
                        for (int k = 0; k < answers[i][j].Length; k++)
                        {
                            testData.Answers[i][j][k] = ((char)((int)'A' + k)).ToString();
                        }
                    }
                }

                FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(fs, testData);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                    return;
                }
                finally
                {
                    fs.Close();
                }
            }
        }

        public TestData LoadTest(string fullPath)
        {
            FileStream fs = null;
            BinaryFormatter formatter = new BinaryFormatter();
            object testDataDeserialized;

            try
            {
                File.SetLastAccessTime(fullPath, DateTime.Now);
                fs = new FileStream(fullPath, FileMode.Open);
                testDataDeserialized = formatter.Deserialize(fs);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }

            if (testDataDeserialized is TestData testData)
            {
                return testData;
            }

            return null;
        }
    }
}