using System;
using System.Collections.Specialized;

namespace QM
{
    /// <summary>
    /// Represents a single data pack send by a client.
    /// </summary>
    public class Record
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string[] Answers { get; set; }
        public int GroupId { get; set; }
        public string Ip { get; set; }
        public int Score { get; set; }
        public int FairPlayStatus { get; set; }
        public DateTime TimeStamp { get; set; }

        public Record(NameValueCollection nameValueCollection)
        {
            if (nameValueCollection.Count < 3)
            {
                Name = null;
                Surname = null;
                GroupId = -1;

                return;
            }

            Name = nameValueCollection.Get("name");
            Surname = nameValueCollection.Get("surname");
            Answers = new string[nameValueCollection.Count - 3];

            if(int.TryParse(nameValueCollection.Get("group"), out int groupID))
            {
                // NOTE: Group number recieved from client is from <1,2,...> range
                GroupId = groupID - 1;
            }
            else
            {
                GroupId = -1;
                return;
            }

            for (int i = 0; i < Answers.Length; i++)
            {
                Answers[i] = nameValueCollection[i + 3];
            }
        }

        public string PrintToCSV()
        {
            return Name + ";" + Surname + ";" + Score.ToString() + ";";
        }

        public override string ToString()
        {
            return Name + " " + Surname + " " + Score.ToString();
        }
    }
}