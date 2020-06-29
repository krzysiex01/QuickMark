 namespace QM
{
    /// <summary>
    /// Stores selected TestData values, ready to be sent to a particular client.
    /// </summary>
    public class TestSheetData
    {
        public string[][] Answers { get; set; }
        public string[] Questions { get; set; }
        public int GroupId { get; set; }
        public int NumberOfGroups { get; set; }
        public bool DigitalTest { get; set; }
    }
}