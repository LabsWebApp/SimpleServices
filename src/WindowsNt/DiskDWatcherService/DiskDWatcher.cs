using System;
using System.IO;
using System.ServiceProcess;

namespace DiskDWatcherService
{
    public partial class DiskDWatcher : ServiceBase
    {
        public const string LogFile = @"С:\history.log";

        private readonly StreamWriter _writer;
        private readonly FileSystemWatcher _watcher;
        private void WriteText(string text = "")
        {
            _writer.WriteLine(text);
            _writer.Flush();
        }

        public DiskDWatcher()
        {
            InitializeComponent();
            var file = new FileInfo(LogFile);
            _writer = file.CreateText();
            _watcher = new FileSystemWatcher(@"D:\");
            //{
            //    NotifyFilter = NotifyFilters.DirectoryName
            //                   | NotifyFilters.FileName
            //                   | NotifyFilters.LastWrite
            //                   | NotifyFilters.Security
            //                   | NotifyFilters.Size,
            //    EnableRaisingEvents = true,
            //    Filter = "*",
            //    IncludeSubdirectories = true
            //};

            //_watcher.IncludeSubdirectories = true;
            //_watcher.EnableRaisingEvents = true;
            //_watcher.NotifyFilter = NotifyFilters.DirectoryName
            //                        | NotifyFilters.FileName
            //                        | NotifyFilters.LastWrite
            //                        | NotifyFilters.Security
            //                        | NotifyFilters.Size;

            _watcher.Created += Watcher_Changed;
            _watcher.Deleted += Watcher_Changed;
            _watcher.Renamed += Watcher_Renamed;
            _watcher.Changed += Watcher_Changed;
            _watcher.Error += Watcher_Error;
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e) =>
            WriteText($"[{DateTime.Now.ToLongTimeString()}] Файл(директория) - \"{e.FullPath}\" был(а) переименована (старое имя: {e.OldName}), тип изменения: {e.ChangeType}");

        private void Watcher_Error(object sender, ErrorEventArgs e) =>
            WriteText($"[{DateTime.Now.ToLongTimeString()}] На диске D: произошла ошибка: {e.GetException().Message}");

        private void Watcher_Changed(object sender, FileSystemEventArgs e) =>
            WriteText($"[{DateTime.Now.ToLongTimeString()}] Файл(директория) - \"{e.FullPath}\" был(а) изменена, тип изменения: {e.ChangeType}");

        protected override void OnStart(string[] args)
        {
            _watcher.EnableRaisingEvents = true;
            var time = DateTime.Now;
            WriteText($"=====Служба слежением за диском D: начала работу в {time.ToShortDateString()} - {time.ToLongTimeString()}=====");
        }


        protected override void OnStop()
        {
            _watcher.EnableRaisingEvents = false;
            var time = DateTime.Now;
            WriteText($"Служба слежением за диском D: закончила работу в {time.ToShortDateString()} - {time.ToLongTimeString()}");
            WriteText();
        }

        protected override void OnShutdown() => OnStop();

        protected override void OnPause() => OnStop();
        
        protected override void OnContinue() => OnStart(null);
    }
}
