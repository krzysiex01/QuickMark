using Microsoft.VisualStudio.TestTools.UnitTesting;
using QM;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Tests
{
    [TestClass()]
    public class TestManagerTests
    {
        private string _testIp = "192.168.0.1";
        private TestManager CreateTestManager()
        {
            TestManager testManager = new TestManager();
            testManager.Settings = new ApplicationSettings();
            testManager.NumberOfGroups = 1;
            testManager.Questions = new string[][] { new string[] { "q1", "q2", "q3" } };
            testManager.Answers = new string[][][] {
            new string[][] {
                 new string[] { "a1", "a2", "a3" } },
                 new string[][] { new string[] { "a1", "a2", "a3" } },
                 new string[][] { new string[] { "a1", "a2", "a3" } }
            };
            testManager.CorrectAnswers = new string[][] { new string[] { "a1", "a2", "a3" } };
            return testManager;
        }

        [TestMethod()]
        public void AddValidRecordWithCorrectAnswers()
        {
            TestManager testManager = CreateTestManager();
            NameValueCollection nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("name", "testName");
            nameValueCollection.Add("surname", "testSurname");
            nameValueCollection.Add("group", "1");
            nameValueCollection.Add("q1", "a1");
            nameValueCollection.Add("q2", "a2");
            nameValueCollection.Add("q3", "a3");
            string output = testManager.AddRecord(nameValueCollection, _testIp);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Contains("3/3"));
        }

        [TestMethod()]
        public void AddValidRecordWithIncorrectAnswers()
        {

            TestManager testManager = CreateTestManager();
            NameValueCollection nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("name", "testName");
            nameValueCollection.Add("surname", "testSurname");
            nameValueCollection.Add("group", "1");
            nameValueCollection.Add("q1", "a2");
            nameValueCollection.Add("q2", "a1");
            nameValueCollection.Add("q3", "a2");
            string output = testManager.AddRecord(nameValueCollection, _testIp);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Contains("0/3"));
        }

        [TestMethod()]
        public void AddValidRecordWithNotExistingAnswers()
        {

            TestManager testManager = CreateTestManager();
            NameValueCollection nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("name", "testName");
            nameValueCollection.Add("surname", "testSurname");
            nameValueCollection.Add("group", "1");
            nameValueCollection.Add("q1", "customAnswer");
            nameValueCollection.Add("q2", "customAnswer");
            nameValueCollection.Add("q3", "customAnswer");
            string output = testManager.AddRecord(nameValueCollection, _testIp);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Contains("0/3"));
        }

        [TestMethod()]
        public void AddRecordWithToManyAnswers()
        {

            TestManager testManager = CreateTestManager();
            NameValueCollection nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("name", "testName");
            nameValueCollection.Add("surname", "testSurname");
            nameValueCollection.Add("group", "1");
            nameValueCollection.Add("q1", "customAnswer");
            nameValueCollection.Add("q2", "customAnswer");
            nameValueCollection.Add("q3", "customAnswer");
            nameValueCollection.Add("q4", "customAnswer");
            string output = testManager.AddRecord(nameValueCollection, _testIp);

            Assert.IsNull(output);
        }

        [TestMethod()]
        public void AddRecordWithNoAnswers()
        {

            TestManager testManager = CreateTestManager();
            NameValueCollection nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("name", "testName");
            nameValueCollection.Add("surname", "testSurname");
            nameValueCollection.Add("group", "1");
            string output = testManager.AddRecord(nameValueCollection, _testIp);

            Assert.IsNull(output);
        }

        [TestMethod()]
        public void AddRecordWithNoName()
        {

            TestManager testManager = CreateTestManager();
            NameValueCollection nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("surname", "testSurname");
            nameValueCollection.Add("group", "1");
            nameValueCollection.Add("q1", "customAnswer");
            nameValueCollection.Add("q2", "customAnswer");
            nameValueCollection.Add("q3", "customAnswer");
            string output = testManager.AddRecord(nameValueCollection, _testIp);

            Assert.IsNull(output);
        }

        [TestMethod()]
        public void AddRecordWithNoSurname()
        {

            TestManager testManager = CreateTestManager();
            NameValueCollection nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("name", "testName");
            nameValueCollection.Add("group", "1");
            nameValueCollection.Add("q1", "customAnswer");
            nameValueCollection.Add("q2", "customAnswer");
            nameValueCollection.Add("q3", "customAnswer");
            string output = testManager.AddRecord(nameValueCollection, _testIp);

            Assert.IsNull(output);
        }

        [TestMethod()]
        public void AddEmptyRecord()
        {
            TestManager testManager = CreateTestManager();
            NameValueCollection nameValueCollection = new NameValueCollection();
            string output = testManager.AddRecord(nameValueCollection, _testIp);

            Assert.IsNull(output);
        }

        public void AddRecordWithInvalidGroup()
        {
            TestManager testManager = CreateTestManager();
            NameValueCollection nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("name", "testName");
            nameValueCollection.Add("surname", "testSurname");
            nameValueCollection.Add("group", "20");
            nameValueCollection.Add("q1", "a1");
            nameValueCollection.Add("q2", "a2");
            nameValueCollection.Add("q3", "a3");
            string output = testManager.AddRecord(nameValueCollection, _testIp);

            Assert.IsNotNull(output);
        }
    }
}