using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM
{
    public class ServiceProviderWrapper : IServiceProvider
    {
        public ApplicationSettings GetApplicationSettings { get => ServiceProvider.GetApplicationSettings; set => ServiceProvider.GetApplicationSettings = value; }

        public bool CheckMinRequirements()
        {
            return ServiceProvider.CheckMinRequirements();
        }

        public bool CheckProgramFilesConsistency()
        {
            return CheckProgramFilesConsistency();
        }
    }
}
