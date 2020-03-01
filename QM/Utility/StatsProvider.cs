using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM
{
    /// <summary>
    /// Provides useful sets of methods for calculating basic statistics from given records.
    /// </summary>
    class StatsProvider
    {
        public StatsProvider(Record[] records, string[][] correctAnswers)
        {
            this.records = records;
            this.correctAnswers = correctAnswers;
        }

        private Record[] records;
        public string[][] correctAnswers;

        public double GetAverageScore()
        {
            if (records.Length == 0)
            {
                return 0;
            }

            return records.Average((Record r) => { return r.Score; });
        }

        public double GetAverageScore(int groupId)
        {
            var res = records.Where((Record r) => { return r.GroupId == groupId; });
            if (res.Count() == 0)
            {
                return 0;
            }
            else
            {
                return res.Average((Record r) => { return r.Score; });
            }
        }

        public IEnumerable<Record> GetTopStudents(int count)
        {
            if (count > records.Length)
            {
                return null;
            }

            return records.OrderBy((Record r) => { return r.Score; }).Take(count);
        }

        public IEnumerable<Record> GetWorstStudents(int count)
        {
            if (count > records.Length)
            {
                return null;
            }

            return records.OrderByDescending((Record r) => { return r.Score; }).Take(count);
        }

        public double[] GetQuestionsAverageScore(int groupId)
        {
            int[] scores = new int[correctAnswers[groupId].Length];
            int studentsCounter = 0;

            foreach (Record record in records)
            {
                if (record.GroupId == groupId)
                {
                    studentsCounter++;
                    for (int i = 0; i < record.Answers.Length; i++)
                    {
                        if (record.Answers[i] == correctAnswers[groupId][i])
                        {
                            scores[i] += 1;
                        }
                    }
                }
            }

            if (studentsCounter == 0)
            {
                return scores.Select<int,double>((int s) => { return (double)s; }).ToArray();
            }

            return scores.Select<int, double>((int s) => { return (double)s / (double)studentsCounter; }).ToArray();
        }
        
        public int[] GetMostDifficultQuestionsIndexes(int groupId, int count)
        {
            double[] avgScores = GetQuestionsAverageScore(groupId);
            int[] res = new int[avgScores.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = i;
            }

            Array.Sort(avgScores, res);

            return res.Take(count).ToArray();
        }
    }
}
