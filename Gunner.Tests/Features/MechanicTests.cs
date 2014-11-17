using System;
using System.Threading.Tasks;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using FluentAssertions;
using Gunner.Engine;
using Gunner.Tests.Mocks;
using NUnit.Framework;
using NetMQ;

namespace Gunner.Tests.Features
{
    public class MechanicTests
    {
        [UseApprovalSubdirectory("approvals")]
        [UseReporter(typeof(DiffReporter))]
        public class WhenListeningAndSending
        {

            private MockLogWriter listenwriter;
            private MockLogWriter sendwriter;

            public WhenListeningAndSending()
            {
                listenwriter = new MockLogWriter(false);
                sendwriter = new MockLogWriter(false);
                var listenTask = Task.Run(() => new Mechanic.Bootstrapper(listenwriter).Start("listen", "-r", "{message} received", "-c", "3", "-p", "9090"));
                var sendTask = Task.Run(() => new Mechanic.Bootstrapper(sendwriter).Start("send", "-p", "9090", "-w", "0", "-s", "127.0.0.1", "-m", "hello1|hello2|hello3"));
                Task.WaitAll(listenTask, sendTask);                
            }

            [Test]
            public void ListenerShouldListenAndRespondToMessagesOnAPort()
            {
                ApprovalTests.Approvals.VerifyAll(listenwriter.ReadLines(), "listenwriter");
            }
            
            [Test]
            public void SenderShouldSendMessagesToServerAndPortAndReceiveResponses()
            {
                ApprovalTests.Approvals.VerifyAll(sendwriter.ReadLines(), "sendwriter");
            }

        }


    }
}
