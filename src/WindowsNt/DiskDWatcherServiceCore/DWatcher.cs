using System.ServiceProcess;

namespace DiskDWatcherServiceCore;

public class DWatcher : ServiceBase
{
    public const string LogFile = @"C:\history.log",
        Disk = @"D:\";

    private readonly object _locker = new();
    private readonly StreamWriter? _writer;
    private readonly FileSystemWatcher? _watcher;

    private async Task WriteTextAsync(string text = "") => await Task.Run(() =>
    {
        lock (_locker)
        {
            _writer?.WriteLine(text);
            _writer?.FlushAsync();
        }
    });

    public DWatcher()
    {
        if (!Directory.Exists(Disk)) return;

        var file = new FileInfo(LogFile);
        _writer = file.AppendText();
        _watcher = new FileSystemWatcher(Disk)
        {
            NotifyFilter = NotifyFilters.DirectoryName
                           | NotifyFilters.FileName
                           | NotifyFilters.LastWrite
                           | NotifyFilters.Security
                           | NotifyFilters.Size,
            EnableRaisingEvents = true,
            Filter = "*",
            IncludeSubdirectories = true
        };

        _watcher.Created += Watcher_Created;
        _watcher.Deleted += Watcher_Deleted;
        _watcher.Changed += Watcher_Changed;
        _watcher.Error += Watcher_Error;
        _watcher.Renamed += Watcher_Renamed;
    }

    private async void Watcher_Deleted(object sender, FileSystemEventArgs e)
    {
        if (e.FullPath.ToUpper().StartsWith(@"D:\$RECYCLE.BIN\")) return;
        await WriteTextAsync($"[{DateTime.Now.ToLongTimeString()}] Файл(директория) - \"{e.FullPath}\" был(а) удален(а)");
    }

    private async void Watcher_Renamed(object sender, RenamedEventArgs e) =>
        await WriteTextAsync($"[{DateTime.Now.ToLongTimeString()}] Файл(директория) - \"{e.FullPath}\" был(а) переименован(а) [старое имя: \"{e.OldName}\"]");

    private async void Watcher_Error(object sender, ErrorEventArgs e) =>
        await WriteTextAsync($"[{DateTime.Now.ToLongTimeString()}] На диске {Disk} произошла ошибка: {e.GetException().Message}");
    private async void Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        if (e.FullPath.ToUpper().StartsWith(@"D:\$RECYCLE.BIN\")) return;
        await WriteTextAsync(
            $"[{DateTime.Now.ToLongTimeString()}] Файл(директория) - \"{e.FullPath}\" был(а) изменен(а), тип изменения: {e.ChangeType}");
    }

    private async void Watcher_Created(object sender, FileSystemEventArgs e)
    {
        if (e.FullPath.ToUpper().StartsWith(@"D:\$RECYCLE.BIN\")) return;
        await WriteTextAsync($"[{DateTime.Now.ToLongTimeString()}] Файл(директория) - \"{e.FullPath}\" был(а) создан(а)");
    }

    protected override async void OnStart(string[] args)
    {
        _watcher!.EnableRaisingEvents = true;
        var time = DateTime.Now;
        await WriteTextAsync($"*****Служба слежением за диском {Disk} начала работу {time.ToShortDateString()} в {time.ToLongTimeString()}*****");
    }


    protected override async void OnStop()
    {
        _watcher!.EnableRaisingEvents = false;
        var time = DateTime.Now;
        await WriteTextAsync($"*****Служба слежением за диском {Disk} закончила работу {time.ToShortDateString()} в {time.ToLongTimeString()}*****");
        await WriteTextAsync();
    }

    protected override void OnShutdown() => OnStop();

    protected override void OnPause() => OnStop();

    protected override void OnContinue() => OnStart(new[]{string.Empty});
}