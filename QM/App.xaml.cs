using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QM
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (ServiceProvider.CheckMinRequirements() && ServiceProvider.CheckProgramFilesConsistency())
            {
                ApplicationView window = new ApplicationView();

                var viewModel = new ApplicationViewModel();
          
                window.DataContext = viewModel;
                window.Show();
            }
            else
            {
                MessageBox.Show("Error occured. Please reinstall the program.");
                Application.Current.Shutdown();
            }
        }
    }
}
