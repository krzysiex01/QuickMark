using System;

namespace QM
{
    /// <summary>
    /// Stores all information about a test. Used by TestManager to create TestSheetData.
    /// </summary>
    [Serializable]
    public class TestData
    {
        public string[][] CorrectAnswers { get; set; }
        public string[][][] Answers { get; set; }
        public string[][] Questions { get; set; }
        public int NumberOfGroups { get; set; }
        public bool AutoGroups { get; set; }
        public bool DigitalTest { get; set; }
        public string TestId { get; set; }
    }
}