using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace QM.TestMaker.Tests
{
    [TestClass]
    public class TestMakerViewModelTests
    {
        private TestManager GetTestManagerForTesting(ApplicationSettings applicationSettings)
        {
            TestManager testManager = new TestManager(applicationSettings);
            return testManager;
        }
        private ApplicationSettings GetApplicationSettingsForTesting()
        {
            ApplicationSettings applicationSettings = new ApplicationSettings
            {
                Prefixes = new string[] { }
            };
            return applicationSettings;
        }

        private class TestServiceProvider : IServiceProvider
        {
            private ApplicationSettings applicationSettings;
            public ApplicationSettings GetApplicationSettings { get => applicationSettings; set => applicationSettings = value; }
            public bool CheckMinRequirements() => true;
            public bool CheckProgramFilesConsistency() => true;
        }
        private TestMakerViewModel CreateTestMakerViewModelClass()
        {
            IServiceProvider serviceProvider = new TestServiceProvider();
            serviceProvider.GetApplicationSettings = GetApplicationSettingsForTesting();
            TestManager testManager = GetTestManagerForTesting(serviceProvider.GetApplicationSettings);
            EventHandler<IPageViewModel> eventHandler = null;
            return new TestMakerViewModel(eventHandler,testManager,serviceProvider);
        }

        [TestMethod]
        public void StartServer()
        {
            TestMakerViewModel testMakerViewModel = CreateTestMakerViewModelClass();
            testMakerViewModel.StartCommand.Execute(null);
            Assert.IsTrue(testMakerViewModel.IsServerStarted);
        }


        [TestMethod]
        public void StartAndStopServer()
        {
            TestMakerViewModel testMakerViewModel = CreateTestMakerViewModelClass();

            testMakerViewModel.StartCommand.Execute(null);
            Assert.IsTrue(testMakerViewModel.IsServerStarted);
            testMakerViewModel.StartCommand.Execute(null);
            Assert.IsTrue(testMakerViewModel.IsServerPaused);
        }

        [TestMethod]
        public void ClearRecordsWithServerRunning()
        {
            TestMakerViewModel testMakerViewModel = CreateTestMakerViewModelClass();
            var nameValueCollection = new System.Collections.Specialized.NameValueCollection();
            nameValueCollection.Add("name", "TestName");
            nameValueCollection.Add("surname", "testSurname");
            nameValueCollection.Add("q1", "someAswer");
            testMakerViewModel.Records.Add(new Record(nameValueCollection));

            testMakerViewModel.StartCommand.Execute(null);
            testMakerViewModel.ResetCommand.Execute(null);
            testMakerViewModel.StartCommand.Execute(null);

            Assert.AreEqual(0, testMakerViewModel.Records.Count);
            Assert.IsTrue(testMakerViewModel.IsServerStarted);
        }

        [TestMethod]
        public void ClearRecordsWithServerStopped()
        {
            TestMakerViewModel testMakerViewModel = CreateTestMakerViewModelClass();
            var nameValueCollection = new System.Collections.Specialized.NameValueCollection();
            nameValueCollection.Add("name", "TestName");
            nameValueCollection.Add("surname", "testSurname");
            nameValueCollection.Add("q1", "someAswer");
            testMakerViewModel.Records.Add(new Record(nameValueCollection));

            testMakerViewModel.StartCommand.Execute(null);
            testMakerViewModel.StartCommand.Execute(null);
            testMakerViewModel.ResetCommand.Execute(null);

            Assert.AreEqual(0, testMakerViewModel.Records.Count);
            Assert.IsTrue(testMakerViewModel.IsServerPaused);
        }
    }
}