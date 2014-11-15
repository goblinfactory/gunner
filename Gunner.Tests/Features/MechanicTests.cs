using System;
using System.Threading.Tasks;
using Gunner.Engine;
using Gunner.Tests.Mocks;
using NUnit.Framework;
using NetMQ;

namespace Gunner.Tests.Features
{
    public class ClientServerTests
    {
        [Test]
        public void SendReceiveTests()
        {
            var listenwriter = new MockLogWriter(false);
            var sendwriter = new MockLogWriter(false);
            var listenTask = Task.Run(() => new Mechanic.Bootstrapper(listenwriter).Start("listen", "-r", "{message} received", "-c", "3", "-p", "9090"));
            var sendTask = Task.Run(() => new Mechanic.Bootstrapper(sendwriter).Start("send", "-p", "9090", "-w", "0", "-s", "127.0.0.1", "-m", "hello1|hello2|hello3"));
            Task.WaitAll(listenTask, sendTask);
            Console.WriteLine("listener output");
            listenwriter.ReadLines().ForEach(Console.WriteLine);
            Console.WriteLine();
            Console.WriteLine("sender output");
            sendwriter.ReadLines().ForEach(Console.WriteLine);
        }

    }
}
