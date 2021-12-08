using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;

namespace DiskDWatcherService
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            Installers.Add(new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            });
            Installers.Add(new ServiceInstaller
            {
                ServiceName = "dwatcher",
                DisplayName = "____Служба слежением за диском D:____",
                Description = "Служба отслеживает и протоколирует изменения в файловой системе диска D:",
                StartType = ServiceStartMode.Automatic
            });
        }
    }
}
