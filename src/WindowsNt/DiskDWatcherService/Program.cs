﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DiskDWatcherService
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new DWatcher()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
