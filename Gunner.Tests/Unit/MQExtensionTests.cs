using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Gunner.Tests.Internal;
using NUnit.Framework;
using NetMQ;
using Gunner.Engine;

namespace Gunner.Tests.Unit
{
    public class MQExtensionTests
    {
        [Test]
        public void ShouldBeAbleToSendAndRecieveGenericMessagesUsingProtobufSerialisation()
        {
            using (NetMQContext ctx = NetMQContext.Create())
            {
                using (var server = ctx.CreateResponseSocket())
                {
                    server.Bind("tcp://127.0.0.1:5556");
                    using (var client = ctx.CreateRequestSocket())
                    {
                        var testSubmission = new TestSubmission() { Name = "Fred", Age = 5, Answers = new[] { "apples", "pears", "oranges" } };
                        client.Connect("tcp://127.0.0.1:5556");
                        Console.WriteLine("submitting test for student:{0}", testSubmission.Name);
                        client.SendT(testSubmission);
                        // processing test scores...
                        var test = server.ReceiveT<TestSubmission>();
                        test.ShouldBeEquivalentTo(testSubmission);
                        Console.WriteLine("received test for student:{0}",test.Name);
                        

                        var testscore = new TestScore() {Name = test.Name, Score = "A+"};
                        Console.WriteLine("sending test results back");
                        server.SendT(testscore);

                        var mytestScore = client.ReceiveT<TestScore>();
                        mytestScore.ShouldBeEquivalentTo(testscore);
                        Console.WriteLine("recieved test score, I got an '{0}'!",mytestScore.Score);
                    }
                }
            }
        }
    }

    public class TestSubmission
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string[] Answers { get; set; }
    }

    public class TestScore
    {
        public string Name { get; set; }
        public string Score { get; set; }
    }

}
