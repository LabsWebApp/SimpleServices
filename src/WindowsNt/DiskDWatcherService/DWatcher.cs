using System;
using System.IO;
using System.ServiceProcess;

namespace DiskDWatcherService
{
    public partial class DWatcher : ServiceBase
    {
        public const string LogFile = @"C:\history.log",
            Disk = @"D:\";

        private readonly StreamWriter _writer;
        private readonly FileSystemWatcher _watcher;
        private void WriteText(string text = "")
        {
            _writer.WriteLine(text);
            _writer.Flush();
        }

        public DWatcher()
        {
            InitializeComponent();
            var file = new FileInfo(LogFile);
            _writer = file.AppendText();

            if (!Directory.Exists(Disk))
            {
                WriteText($"*****Служба слежением за диском {Disk} не может начать работать, так {Disk} отсутствует или скрыт в файловой системе*****");
                _writer.Dispose();
                return;
            }

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

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.ToUpper().StartsWith(@"D:\$RECYCLE.BIN\")) return;
            WriteText($"[{DateTime.Now.ToLongTimeString()}] Файл(директория) - \"{e.FullPath}\" был(а) удален(а)");
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e) =>
            WriteText($"[{DateTime.Now.ToLongTimeString()}] Файл(директория) - \"{e.FullPath}\" был(а) переименован(а) [старое имя: \"{e.OldName}\"]");

        private void Watcher_Error(object sender, ErrorEventArgs e) =>
            WriteText($"[{DateTime.Now.ToLongTimeString()}] На диске {Disk} произошла ошибка: {e.GetException().Message}");
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.ToUpper().StartsWith(@"D:\$RECYCLE.BIN\")) return;
            WriteText(
                $"[{DateTime.Now.ToLongTimeString()}] Файл(директория) - \"{e.FullPath}\" был(а) изменен(а), тип изменения: {e.ChangeType}");
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.ToUpper().StartsWith(@"D:\$RECYCLE.BIN\")) return;
            WriteText($"[{DateTime.Now.ToLongTimeString()}] Файл(директория) - \"{e.FullPath}\" был(а) создан(а)");
        }

        protected override void OnStart(string[] args)
        {
            _watcher.EnableRaisingEvents = true;
            var time = DateTime.Now;
            WriteText($"*****Служба слежением за диском {Disk} начала работу {time.ToShortDateString()} в {time.ToLongTimeString()}*****");
        }


        protected override void OnStop()
        {
            _watcher.EnableRaisingEvents = false;
            var time = DateTime.Now;
            WriteText($"*****Служба слежением за диском {Disk} закончила работу {time.ToShortDateString()} в {time.ToLongTimeString()}*****");
            WriteText();
        }

        protected override void OnShutdown() => OnStop();

        protected override void OnPause() => OnStop();

        protected override void OnContinue() => OnStart(null);
    }
}
