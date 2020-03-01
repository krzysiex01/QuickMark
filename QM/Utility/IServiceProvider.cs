using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM
{
    public interface IServiceProvider
    {
        ApplicationSettings GetApplicationSettings { get; set; }
        bool CheckProgramFilesConsistency();
        bool CheckMinRequirements();
     }
}
