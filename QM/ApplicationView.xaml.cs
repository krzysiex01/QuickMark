using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QM
{
    /// <summary>
    /// Logika interakcji dla klasy ApplicationView.xaml
    /// </summary>
    public partial class ApplicationView : MetroWindow, ICloseable
    {
        public ApplicationView()
        {
            InitializeComponent();
        }
    }

    public interface ICloseable
    {
        void Close();
    }

}
