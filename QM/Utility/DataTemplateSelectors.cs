using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QM.TestCreator
{
    /// <summary>
    /// Returns template according to given string.
    /// </summary>
    public class TestTypeDisplaySelector : DataTemplateSelector
    {
        public DataTemplate DefaultDataTemplate { get; set; }
        public DataTemplate DigitalTestDataTemplate { get; set; }
        public DataTemplate ExamTestDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is string s)
            {
                if (s == "DigitalTestDataTemplate")
                {
                    return DigitalTestDataTemplate;
                }
                else if (s == "ExamTestDataTemplate")
                {
                    return ExamTestDataTemplate;
                }
                else
                {
                    return DefaultDataTemplate;
                }
            }
            else
            {
                return DefaultDataTemplate;
            }
        }
    }
}

namespace QM.TestMaker
{
    /// <summary>
    /// Returns template according to FairPlayStatus value.
    /// </summary>
    /// <see cref="TestManager.CalculateFairPlayStatus(Record, Record)"/>
    public class FairPlayStatusDisplaySelector : DataTemplateSelector
    {
        public DataTemplate DefaultDataTemplate { get; set; }
        public DataTemplate VerifiedDataTemplate { get; set; }
        public DataTemplate AlertDataTemplate { get; set; }
        public DataTemplate WarningDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is int v)
            {
                if (v == int.MaxValue)
                {
                    return DefaultDataTemplate;
                }
                else if (v < 0 && v > -500)
                {
                    return WarningDataTemplate;
                }
                else if (v >= 0)
                {
                    return VerifiedDataTemplate;
                }
                else
                {
                    return AlertDataTemplate;
                }
            }
            else
            {
                return DefaultDataTemplate;
            }
        }
    }
}
