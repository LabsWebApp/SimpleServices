using DiskDWatcherServiceCore;
using Topshelf;

HostFactory.Run(x =>
    {
        x.Service<DWatcher>();
        x.EnableServiceRecovery(r => r.RestartService(TimeSpan.FromSeconds(10)));
        x.SetServiceName("dwatchercore");
        x.SetDisplayName($"____Служба слежением за диском {DWatcher.Disk}____");
        x.SetDescription($"Служба отслеживает и протоколирует изменения в файловой системе диска {DWatcher.Disk}");
        x.StartAutomatically();
    }
);