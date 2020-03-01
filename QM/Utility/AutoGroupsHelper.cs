using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM
{
    /// <summary>
    /// Provides methods converting one-group test to multi-group test by shuffling the order of questions and answers.
    /// </summary>
    class AutoGroupsHelper
    {
        /// <summary>
        /// Creates new groups by changing the order of questions and answers in existing base version of test.
        /// Number of created groups is equal to GroupLimit value defined in ApplicationSettings class.
        /// </summary>
        /// <returns>Numbers of created groups.</returns>
        public int ShuffleTest(string[][] answers, string[] correctAnswers, string[] questions, out string[][][] mixedAnswers, out string[][] mixedCorrectAnswers,
            out string[][] mixedQuestions, bool mixAnswersOrder = true, bool mixQuestionsOrder = true)
        {
            Random random = new Random();
            string[][] tmpAnswers = answers;
            string[] tmpCorrectAnswers = correctAnswers;
            string[] tmpQuestions = questions;
            mixedAnswers = new string[ServiceProvider.GetApplicationSettings.GroupsLimit][][];
            mixedCorrectAnswers = new string[ServiceProvider.GetApplicationSettings.GroupsLimit][];
            mixedQuestions = new string[ServiceProvider.GetApplicationSettings.GroupsLimit][];

            for (int i = 0; i < ServiceProvider.GetApplicationSettings.GroupsLimit; i++)
            {
                mixedAnswers[i] = (string[][])tmpAnswers.Clone();
                mixedCorrectAnswers[i] = (string[])tmpCorrectAnswers.Clone();
                mixedQuestions[i] = (string[])tmpQuestions.Clone();

                for (int j = 0; j < tmpQuestions.Length; j++)
                {
                    int swapWith;
                    if (mixQuestionsOrder)
                    {
                        swapWith = random.Next(tmpQuestions.Length);
                        (tmpQuestions[j], tmpQuestions[swapWith]) = (tmpQuestions[swapWith], tmpQuestions[j]);
                        (tmpCorrectAnswers[j], tmpCorrectAnswers[swapWith]) = (tmpCorrectAnswers[swapWith], tmpCorrectAnswers[j]);
                        (tmpAnswers[j], tmpAnswers[swapWith]) = (tmpAnswers[swapWith], tmpAnswers[j]);
                    }
                    if (mixAnswersOrder)
                    {
                        tmpAnswers[j] = (string[])tmpAnswers[j].Clone();
                        for (int k = 0; k < tmpAnswers[j].Length; k++)
                        {
                            swapWith = random.Next(tmpAnswers[j].Length);
                            (tmpAnswers[j][k], tmpAnswers[j][swapWith]) = (tmpAnswers[j][swapWith], tmpAnswers[j][k]);
                        }
                    }
                }
            }

            return ServiceProvider.GetApplicationSettings.GroupsLimit;
        }

    }
}
