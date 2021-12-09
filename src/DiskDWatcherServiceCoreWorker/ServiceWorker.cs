namespace DiskDWatcherServiceCoreWorker;

public class ServiceWorker : BackgroundService
{
    public const string LogFile = @"C:\history.log", Disk = @"D:\";

    public bool CanWork => _watcher is not null;
    public List<(string Info, DateTime Time)>? Works { get; private set; }

    private void AddWork(string info) => Works?.Add(new()
    {
        Info = info,
        Time = DateTime.Now
    });

    private readonly ILogger<ServiceWorker> _logger;
    private readonly FileSystemWatcher? _watcher;

    public ServiceWorker(ILogger<ServiceWorker> logger)
    {
        _logger = logger;
        if (!Directory.Exists(Disk)) return;

        Works = new();
        _watcher = new FileSystemWatcher(Disk)
        {
            NotifyFilter = NotifyFilters.DirectoryName
                           | NotifyFilters.FileName
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

    private void Watcher_Deleted(object sender, FileSystemEventArgs e)
    {
        if (e.FullPath.ToUpper().StartsWith(@"D:\$RECYCLE.BIN\")) return;
        AddWork($"Файл(директория) - \"{e.FullPath}\" был(а) удален(а)");
    }

    private void Watcher_Renamed(object sender, RenamedEventArgs e) =>
        AddWork($"Файл(директория) - \"{e.FullPath}\" был(а) переименован(а) [старое имя: \"{e.OldName}\"]");

    private void Watcher_Error(object sender, ErrorEventArgs e) =>
        AddWork($"На диске {Disk} произошла ошибка: {e.GetException().Message}");
    private void Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        if (e.FullPath.ToUpper().StartsWith(@"D:\$RECYCLE.BIN\")) return;
        AddWork($"Файл(директория) - \"{e.FullPath}\" был(а) изменен(а), тип изменения: {e.ChangeType}");
    }

    private void Watcher_Created(object sender, FileSystemEventArgs e)
    {
        if (e.FullPath.ToUpper().StartsWith(@"D:\$RECYCLE.BIN\")) return;
        AddWork($"Файл(директория) - \"{e.FullPath}\" был(а) создан(а)");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!CanWork) return;
        _watcher!.EnableRaisingEvents = true;
        while (!stoppingToken.IsCancellationRequested)
        {
            Works?.ForEach(w => _logger.LogInformation(
                "{w.Info} - {w.Time}", w.Info, w.Time));
            Works?.Clear();
            await Task.Delay(1000, stoppingToken);
        }

        _watcher!.EnableRaisingEvents = false;
        Works = null;
    }
}