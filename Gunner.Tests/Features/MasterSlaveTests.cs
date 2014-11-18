using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gunner.Engine;
using Gunner.Tests.Internal;
using Gunner.Tests.Mocks;
using NUnit.Framework;
using NetMQ;

namespace Gunner.Tests.MechanicTests
{
    public class ClientServerTests
    {
        [Test,Category("slow")]
        public void GunnerShouldBeAbleToBeRemotelyStarted()
        {
            //GivenGunnersRunningInSlaveMode(2);
            //var task1 = new bootstrapper().Start("mode","slave", "--port", "9090", "--server", "127.0.0.1");
            //task1.Start();
            //var task2 = new bootstrapper().Start("mode", "slave", "--port", "9090", "--server", "127.0.0.1");
            //task2.Start();
            //var task3 = new bootstrapper().Start(
            //    "--mode", "server", 
            //    "--port", "9090", 
            //    "-w", "http://localhost:8080/", 
            //    "-u", "large1.json,large2.json",
            //    "-n", "latitude",
            //    "--stagger", "100", 
            //    "-s", "10", 
            //    "-p", "0",  
            //    "-i", "10", 
            //    "-e", "30");
            //task3.Start();

            //WhenMasterGunnerStarts();
            //ThenAllSlavesShouldStartAtTheSameTime();
        }

        public void GunnerShouldProduceAConsolidatedReportFromAllSlaves()
        {
            //GivenGunnerRunningInSlaveMode();
            //WhenMasterGunnerStarts();
            //ThenAllSlavesShouldStartAtTheSameTime();
            //AndGunnerMasterShouldBeWaitingForSlavesToFinish();
            //WhenAllTheBatchesAreRecieved();
            //ThenGunnerWillProductAConsolidatedReportFromAllSlaves();
        }


        public void GunnerShouldReceiveCommandLineParamsWhenStartedRemotely()
        {
            //GivenGunnerRunningInSlaveMode();
            //WhenMasterGunnerStarts();
            //ThenSlaveShouldRecieveTheMasterScript();
        }

        public void SimulatePubSubSequenceExpectedFromGunnerMasterSlave()
        {
            // commands require a key (poor man's security)    
            // sequence
            // machine 1) gunner1 -master    (subscribes to the batch channel)
            // machine 2) gunner2 -slave1     (subscribes to command channel) 
            // machine 3) gunner3 -slave2     (subscribes to command channel)
            // Master publishes startcommand to "command" channel
            // slave1 receives command, runs it's first batch1 and publishes batch result to batch channel.
            // slave2 receives command, runs it's first batch2 and publishes batch result to batch channel.
            // master continues processing batches until it receives lastbatch from all the slaves
            // master (gunner1) recieves batch1 ( it is not last batch)
            // master (gunner1) recieves batch2 ( it is not last batch)
            // gunner 2, runs 2nd (last) batch, and publishes Lastbatch result to batch channel.
            // gunner 3, runs 2nd (last) batch, and publishes Lastbatch result to batch channel.
            // master now has received lastbatch from all slaves
            // master collates (combines) results from batches.
        }

        private void GivenGunnersRunningInSlaveMode(int batches)
        {
            //Test.TraceStep(batches);
            //int increment = 5;
            //int start = 10;
            //var batchOptions = new BatchOptions
            //{
            //    Start = 10,
            //    End = start + (increment * (batches - 1)),
            //    Pause = 0,
            //    Gap = 0,
            //    Repeat = 5,
            //    Logfile = null,
            //    Find = Settings.TestFileContains,
            //    Root = Settings.Root,
            //    Increment = increment,
            //    UrlList = Settings.TestFile,
            //    Timeout = 3,
            //    Verbose = false
            //};

            //var logwriter = new MockLogWriter(false);
            //var metricMonitoring = new NullMetricMonitor();
            //var trafficMonitor = new MockTrafficMonitor(100);
            //var urls = new[] { "file1.json", "file2.json" };
            //var downloader = new MockDownloader(new DownloadResult { ErrorCode = null, Success = true });
            //var reporter = new Reporter(batchOptions, logwriter);
            //var gunner = new MachineGun(reporter, downloader, batchOptions, urls, trafficMonitor, metricMonitoring);
            //Task.WaitAll(gunner.Run());
        }

    }
}
