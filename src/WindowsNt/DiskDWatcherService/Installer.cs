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
            var processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;

            var serviceInstaller = new ServiceInstaller();
            serviceInstaller.ServiceName = "DiskDWatcher";
            serviceInstaller.DisplayName = "_____ DISK D WATCHER SERVICE _____";
            serviceInstaller.Description = "Служба слежения за папками и файлами на диске D:";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
