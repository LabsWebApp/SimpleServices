using DiskDWatcherServiceCoreWorker;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<ServiceWorker>();
    })
    .Build();

await host.RunAsync();