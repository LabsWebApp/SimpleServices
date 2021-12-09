using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiskDWatcherServiceCore.Tests;

[TestClass()]
public class DWatcherTests
{
    public TestContext? TestContext { get; set; }

    [TestMethod()]
    public void StartStopTest()
    {
        DWatcher watcher = new();

        try
        {
            if (!watcher.Start(null))
                throw new Exception("Not started");
        }
        catch
        {
            Assert.Fail();
        }
        Thread.Sleep(1000*1);
        try
        {
            watcher.Stop(null);
        }
        catch(Exception ex)
        {
            TestContext?.WriteLine(ex.Message);
            TestContext?.WriteLine(ex.GetType().Name);
            Assert.Fail();
        }
        TestContext?.WriteLine("OK");
        Assert.IsTrue(true);
    }
}