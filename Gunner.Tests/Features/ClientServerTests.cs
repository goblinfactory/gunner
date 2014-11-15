//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using NUnit.Framework;
//using NetMQ;

//namespace Gunner.Tests.MechanicTests
//{
//    public class ClientServerTests
//    {
//        [Test] 
//        // perhaps need a timeout in case something goes wrong?
//        public void ProveZeroMQConceptuallyWillSolveMasterSlave()
//        {
//            using (NetMQContext ctx = NetMQContext.Create())
//            {
//                using (var server = ctx.CreateResponseSocket())
//                {
//                    server.Bind("tcp://127.0.0.1:5556");
//                    using (var client = ctx.CreateRequestSocket())
//                    {
//                        client.Connect("tcp://127.0.0.1:5556");
//                        client.Send("Hello");

//                        string m1 = server.ReceiveString();
//                        Console.WriteLine("From Client: {0}", m1);
//                        server.Send("Hi Back");

//                        string m2 = client.ReceiveString();
//                        Console.WriteLine("From Server: {0}", m2);
//                        Console.ReadLine();
//                    }
//                }
//            }
//        }

//        //public class GunnerShould
//        //{

//        //}

//        //public void GunnerShouldPublishBatchResultsToControllerIPAndPort()
//        //{
            
//        //}

//        public void GunnerShouldBeAbleToBeRemotelyStarted()
//        {
//            GivenGunnerRunningInSlaveMode();
//            WhenMasterGunnerStarts();
//            ThenAllSlavesShouldStartAtTheSameTime();
//        }

//        public void GunnerShouldReceiveCommandLineParamsWhenStartedRemotely()
//        {
//            GivenGunnerRunningInSlaveMode();
//            WhenMasterGunnerStarts();
//            ThenSlaveShouldRecieveTheMasterScript();
//        }
 

//    }
//}
