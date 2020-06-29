using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Windows.Data;
using OfficeOpenXml;
using System.Linq;
using System.Windows;

namespace QM
{
    public interface IManager
    {
        string AddRecord(NameValueCollection nameValueCollection, string ip);

        int NumberOfGroups { get; set; }
    }

    /// <summary>
    /// Stores and manipulates records recived from clients.
    /// Provides methods set up new test (load TestData and generate files that can be send by server)
    /// </summary>
    public class TestManager : IManager
    {
        public ApplicationSettings Settings { get; set; }
        public ObservableCollection<Record> records;
        public int NumberOfGroups { get; set; }
        public string TestId { get; set; }
        private readonly object _recordsLock;
        public string[][] CorrectAnswers { get; set; }
        public string[][][] Answers { get; set; }
        public string[][] Questions { get; set; }
        private bool AutoGroups { get; set; }
        private bool DigitalTest { get; set; }

        public TestManager(ApplicationSettings settings = null)
        {
            Settings = settings;
            records = new ObservableCollection<Record>();
            _recordsLock = new object();
            BindingOperations.EnableCollectionSynchronization(records, _recordsLock); //allows other threads to add elements to the collection
        }
        /// <summary>
        /// <see cref="AutoGroupsHelper.ShuffleTest(string[][], string[], string[], out string[][][], out string[][], out string[][], bool, bool)"/>
        /// </summary>
        private void ShuffleTest(bool mixAnswersOrder, bool mixQuestionsOrder)
        {
            AutoGroupsHelper autoGroupsHelper = new AutoGroupsHelper();
            NumberOfGroups = autoGroupsHelper.ShuffleTest(Answers[0], CorrectAnswers[0], Questions[0],
                out string[][][] answers, 
                out string[][] correctAnswers, 
                out string[][] questions, 
                mixAnswersOrder, mixQuestionsOrder);
            Answers = answers;
            Questions = questions;
            CorrectAnswers = correctAnswers;
        }
        /// <summary>
        /// Uses simple algorithm to compare two Records and estimate similarity of answers.
        /// Also checks if one client have not send more than one set of answers.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        private void CalculateFairPlayStatus(Record r1, Record r2)
        {
            if (r1.Ip == r2.Ip)
            {
                r1.FairPlayStatus -= 500;
                r2.FairPlayStatus -= 500;
            }
            else if (r1.GroupId == r2.GroupId)
            {
                int congruityLevel = (int)r1.Answers.Length;

                for (int i = 0; i < r1.Answers.Length; i++)
                {
                    if (r1.Answers[i] != r2.Answers[i])
                    {
                        congruityLevel -= 10;
                    }

                    if (congruityLevel < 0)
                    {
                        r1.FairPlayStatus += 1;
                        r2.FairPlayStatus += 1;
                        return;
                    }
                }

                r1.FairPlayStatus -= 10 * congruityLevel;
                r2.FairPlayStatus -= 10 * congruityLevel;
            }
            else
            {
                r1.FairPlayStatus += 1;
                r2.FairPlayStatus += 1;
            }
        }
        /// <summary>
        /// Creates directory (if doesn't exist) for storing logs and files created by autosave.
        /// </summary>
        /// <returns>Full path to created directory.</returns>
        private string CreateLogDirectory()
        {
            string folderName = DateTime.Today.ToShortDateString();
            var path = Path.Combine(Settings.LogFolderPath, folderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        /// <summary>
        /// Saves records in application log folder.
        /// </summary>
        public void AutoSaveRecords()
        {
            string path = CreateLogDirectory();

            if (Settings.SaveAsCSV)
            {
                SaveRecordsAs(Path.Combine(path, Path.GetRandomFileName() + ".csv"));
            }

            if (Settings.SaveAsXLSL)
            {
                SaveRecordsAs(Path.Combine(path, Path.GetRandomFileName() + ".xlsx"));
            }
        }
        /// <summary>
        /// Saves records to specified destionation.
        /// </summary>
        /// <param name="fullPath">Full destination path (with file extension).</param>
        public void SaveRecordsAs(string fullPath)
        {
            switch (Path.GetExtension(fullPath))
            {
                case ".xlsx":
                    {
                        using (ExcelPackage excel = new ExcelPackage())
                        {
                            excel.Workbook.Worksheets.Add("Results");
                            excel.Workbook.Worksheets.Add("Detailed results");
                            excel.Workbook.Worksheets.Add("Correct answers");
                            excel.Workbook.Worksheets.Add("Stats");

                            //Worksheet - Results
                            var headerRow = new List<string[]>()
                                {
                                   new string[] { "First Name", "Last Name", "Points", "Result (%)" }
                                };

                            string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";

                            var resultsWorksheet = excel.Workbook.Worksheets["Results"];

                            resultsWorksheet.Cells[headerRange].LoadFromArrays(headerRow);
                            resultsWorksheet.Cells[headerRange].Style.Font.Bold = true;

                            var data = new List<string[]>();

                            for (int i = 0; i < records.Count; i++)
                            {
                                data.Add(new string[] { records[i].Name, records[i].Surname, records[i].Score.ToString(), $"{Math.Round((double)records[i].Score / records[i].Answers.Length * 100, 2)}%" });
                            }

                            resultsWorksheet.Cells[2, 1].LoadFromArrays(data);
                            resultsWorksheet.Cells.AutoFitColumns();

                            //Worksheet - Detailed results
                            headerRow = new List<string[]>()
                                {
                                   new string[] { "First Name", "Last Name", "Points", "Result (%)", "Group", "FairPlayStatus", "Ip", "Time Stamp" }
                                };

                            headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";

                            resultsWorksheet = excel.Workbook.Worksheets["Detailed results"];

                            resultsWorksheet.Cells[headerRange].LoadFromArrays(headerRow);
                            resultsWorksheet.Cells[headerRange].Style.Font.Bold = true;


                            data = new List<string[]>();

                            for (int i = 0; i < records.Count; i++)
                            {
                                data.Add(new string[]
                                {
                                    records[i].Name, records[i].Surname, records[i].Score.ToString(),
                                    $"{Math.Round((double)records[i].Score / records[i].Answers.Length * 100, 2)}%",
                                    (records[i].GroupId + 1).ToString(),records[i].FairPlayStatus.ToString(),records[i].Ip.ToString(),records[i].TimeStamp.ToString()
                                });
                            }

                            resultsWorksheet.Cells[2, 1].LoadFromArrays(data);
                            resultsWorksheet.Cells.AutoFitColumns();

                            //Worksheet - Corrrect answers
                            resultsWorksheet = excel.Workbook.Worksheets["Correct answers"];

                            data = new List<string[]>();

                            for (int i = 0; i < CorrectAnswers.Length; i++)
                            {
                                data.Add(new string[] { $"Correct answers for group {i + 1}" });

                                data.Add(Questions[i]);
                                data.Add(CorrectAnswers[i]);
                            }

                            resultsWorksheet.Cells[2, 1].LoadFromArrays(data);
                            resultsWorksheet.Cells.AutoFitColumns();

                            //Worksheet - Stats
                            StatsProvider statsProvider = new StatsProvider(records.ToArray(), CorrectAnswers);
                            resultsWorksheet = excel.Workbook.Worksheets["Stats"];

                            data = new List<string[]>
                            {
                                new string[] { "Test name:", TestId },
                                new string[] { "Date:", DateTime.Now.ToShortDateString() },
                                new string[] { "Average score:", $"{Math.Round(statsProvider.GetAverageScore(), 2).ToString()}", $"{Math.Round(statsProvider.GetAverageScore() * 100 / CorrectAnswers[0].Length, 2).ToString()}%" }
                            };
                            for (int i = 0; i < CorrectAnswers.Length; i++)
                            {
                                data.Add(new string[] { $"Average score for group {(i+1).ToString()}", $"{Math.Round(statsProvider.GetAverageScore(i), 2).ToString()}", $"{Math.Round(statsProvider.GetAverageScore(i) * 100 / CorrectAnswers[i].Length, 2).ToString()}%" });
                            }
                            data.Add(new string[] { $"Top {Math.Min(records.Count, 5)} students:" });
                            foreach (var record in statsProvider.GetTopStudents(Math.Min(records.Count, 5)))
                            {
                                data.Add(new string[] { record.Name, record.Surname, record.Score.ToString(), $"{Math.Round((double)record.Score * 100 / CorrectAnswers[record.GroupId].Length, 2).ToString()}%" });
                            }

                            data.Add(new string[] { $"Worst {Math.Min(records.Count, 5)} students:" });
                            foreach (var record in statsProvider.GetWorstStudents(Math.Min(records.Count, 5)))
                            {
                                data.Add(new string[] { record.Name, record.Surname, record.Score.ToString(), $"{Math.Round((double)record.Score * 100 / CorrectAnswers[record.GroupId].Length, 2).ToString()}%" });
                            }

                            for (int i = 0; i < CorrectAnswers.Length; i++)
                            {
                                data.Add(new string[] { $"Questions analysis for group {(i+1).ToString()}:" });
                                int j = 0;
                                foreach (double avgScore in statsProvider.GetQuestionsAverageScore(i))
                                {
                                    data.Add(new string[] { $"{j + 1}. {Questions[i][j]}", Math.Round(avgScore * 100, 2).ToString() + "%" });
                                    j++;
                                }
                            }

                            for (int i = 0; i < CorrectAnswers.Length; i++)
                            {
                                data.Add(new string[] { $"Most difficult questions for group {(i+1).ToString()}:" });
                                foreach (int index in statsProvider.GetMostDifficultQuestionsIndexes(i, Math.Min(Questions[i].Length, 5)))
                                {
                                    data.Add(new string[] { $"{(index + 1).ToString()}. {Questions[i][index]}" });
                                }
                            }

                            resultsWorksheet.Cells[1, 1].LoadFromArrays(data);

                            resultsWorksheet.Cells[1, 1, 3, 1].Style.Font.Bold = true;

                            resultsWorksheet.Cells[4 + CorrectAnswers.Length, 1].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                            resultsWorksheet.Cells[4 + CorrectAnswers.Length, 1].Style.Font.Bold = true;

                            resultsWorksheet.Cells[4 + CorrectAnswers.Length + Math.Min(records.Count, 5) + 1, 1].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            resultsWorksheet.Cells[4 + CorrectAnswers.Length + Math.Min(records.Count, 5) + 1, 1].Style.Font.Bold = true;

                            resultsWorksheet.Cells.AutoFitColumns();

                            excel.SaveAs(new FileInfo(fullPath));
                        }
                        break;
                    }
                case ".csv":
                    {
                        using (TextWriter textWriter = File.CreateText(fullPath))
                        {
                            textWriter.Write($"Test name: {TestId} ; \n \n");
                            textWriter.Write($"Date: {DateTime.Now.ToShortDateString()} ; \n \n");
                            textWriter.Write("Answer key: ; \n");
                            for (int i = 0; i < CorrectAnswers.Length; i++)
                            {
                                textWriter.Write($"Group {i + 1} ; \n");

                                textWriter.Write($"Questions: ;");
                                for (int j = 0; j < Questions[i].Length; j++)
                                {
                                    textWriter.Write($"{Questions[i][j]};");
                                }
                                textWriter.Write("\n");

                                textWriter.Write($"Correct answers: ;");
                                for (int j = 0; j < CorrectAnswers[i].Length; j++)
                                {
                                    textWriter.Write($"{CorrectAnswers[i][j]};");
                                }
                                textWriter.Write("\n");
                            }

                            textWriter.Write("\n Results: \n");
                            textWriter.WriteLine("Name; Surname; Points; (%);");
                            foreach (Record record in records)
                            {
                                textWriter.WriteLine(record.PrintToCSV() + $"{Math.Round((double)record.Score / record.Answers.Length * 100, 2)}%;");
                            }

                            textWriter.Write("\n Detailed results: \n");
                            textWriter.WriteLine("Name; Surname; Points; (%); Group; FairPlayStatus; Ip; Time Stamp;");
                            foreach (Record record in records)
                            {
                                textWriter.Write(record.PrintToCSV() + $"{Math.Round((double)record.Score / record.Answers.Length * 100, 2) }%;");
                                textWriter.Write($"{record.GroupId + 1}; {record.FairPlayStatus}; {record.Ip}; {record.TimeStamp}; \n");
                            }

                            textWriter.Write("\n Full answers: \n");
                            foreach (Record record in records)
                            {
                                textWriter.Write(record.Name + ";" + record.Surname + ";");
                                foreach (var answer in record.Answers)
                                {
                                    textWriter.Write(answer + ";");
                                }
                                textWriter.Write("\n");
                            }

                            textWriter.Flush();
                        }
                        break;
                    }
                default:
                    break;
            }
        }
        /// <summary>
        /// Deserializes file to TestData object and assings corresponding fields of deserialized object to current TestManager instance.
        /// </summary>
        /// <remarks>This should be the first method to be called after creating new instance of the TestManager class.</remarks>
        /// <param name="fullPath">Location of the file to be deserialized.</param>
        public bool LoadTestData(string fullPath)
        {
            TestSerializer testSerializer = new TestSerializer();
            TestData testData = testSerializer.LoadTest(fullPath);

            if (testData is null)
            {
                return false;
            }
            else
            {
                AutoGroups = testData.AutoGroups;
                Answers = testData.Answers;
                CorrectAnswers = testData.CorrectAnswers;
                Questions = testData.Questions;
                NumberOfGroups = testData.NumberOfGroups;
                DigitalTest = testData.DigitalTest;
                TestId = testData.TestId;

                if (AutoGroups)
                {
                    ShuffleTest(Settings.MixAnswers, Settings.MixQuestions);
                }

                GenerateTests();
                return true;
            }
        }
        /// <summary>
        /// Creates JSON files containing data neccessary to display tests on clients devices.
        /// Each file represents one version (group) of the test.
        /// </summary>
        /// <remarks>Should not be called before successfull execution of LoadTestData method.</remarks>
        private void GenerateTests()
        {
            var serializer = new JavaScriptSerializer();

            for (int groupId = 0; groupId < NumberOfGroups; groupId++)
            {
                TestSheetData data = new TestSheetData() { Answers = Answers[groupId], Questions = Questions[groupId], GroupId = groupId, DigitalTest = DigitalTest, NumberOfGroups = NumberOfGroups };

                var serializedObject = serializer.Serialize(data);
                using (var s = File.CreateText(Path.Combine(Settings.ServerContentFolderPath, @"Json/", $"data{groupId.ToString()}.json")))
                    s.Write(serializedObject);
            }
        }
        /// <summary>
        /// Calculates result by comparing record.Answers array with CorrectAnswers array.
        /// </summary>
        /// <param name="record">Contains answers recived from a client.</param>
        /// <returns>Recived points.</returns>
        private int GetResult(Record record)
        {
            int res = 0;
            int i = 0;
            foreach (var answer in record.Answers)
            {
                if (answer == CorrectAnswers[record.GroupId][i++])
                {
                    res++;
                }
            }

            return res;
        }
        /// <summary>
        /// Thread safe method adding new Record to collection.
        /// </summary>
        /// <param name="nameValueCollection">Contains answers send by a client.</param>
        /// <param name="ip">IPv4 address of the device that send the data.</param>
        /// <returns>Text that will be send back to the client or null if parametrs are invalid.</returns>
        public string AddRecord(NameValueCollection nameValueCollection, string ip)
        {
            if (nameValueCollection == null)
            {
                return null;
            }

            Record record = new Record(nameValueCollection);

            if (record.Name is null || record.Surname is null || record.GroupId < 0 || record.GroupId > NumberOfGroups || record.Answers.Length != CorrectAnswers[record.GroupId].Length)
            {
                return null;
            }

            record.Score = GetResult(record);
            record.Ip = ip;
            record.TimeStamp = DateTime.Now;

            if (Settings.IgnoreDuplicate)
            {
                record.FairPlayStatus = int.MaxValue;

                lock (_recordsLock)
                {
                    records.Add(record);
                }
                if (Settings.HideResult)
                {
                    return "Answers saved successfully.";
                }
                else
                {
                    return "Answers saved successfully. Your score: " + record.Score.ToString() + "/" + record.Answers.Length;
                }
            }
            else
            {
                foreach (Record r in records)
                {
                    if (r.Name == record.Name && r.Surname == record.Surname)
                    {
                        return "Your answers found on server. You cannot change them.";
                    }

                    if (Settings.IgnoreCheating)
                    {
                        record.FairPlayStatus = int.MaxValue;
                    }
                    else
                    {
                        CalculateFairPlayStatus(record, r);
                    }
                }

                lock (_recordsLock)
                {
                    records.Add(record);
                }

                if (Settings.HideResult)
                {
                    return "Answers saved successfully.";
                }
                else
                {
                    return "Answers saved successfully. Your score: " + record.Score.ToString() + "/" + record.Answers.Length;
                }
            }
        }
    }
}