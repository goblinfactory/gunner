using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Gunner.Engine;
using NUnit.Framework;
using NetMQ;
using NetMQ.zmq;

namespace Gunner.Tests.Unit
{

    [TestFixture]
    public class ShouldBeAbleToDoSimpleGenericTPubSub
    {
        private static decimal _price = 180M;

        [Test]
        public void WhenPublishingAMessageAllSubscribersShouldReceiveMessage()
        {
            using (NetMQContext ctx = NetMQContext.Create())
            {
                using (var publisher = ctx.CreatePublisherSocket())
                {
                    publisher.Bind("tcp://*:5557");

                    // client subscribed to receive updates...
                    Console.WriteLine("subscribing to receive our stock price feed");

                    using (var client1 = ctx.CreateSubscriberSocket())
                    {
                        using (var client2 = ctx.CreateSubscriberSocket())
                        {

                            client1.Connect("tcp://localhost:5557");
                            client2.Connect("tcp://localhost:5557");

                            // subscribe to channel
                            client1.Subscribe("");
                            client2.Subscribe("");
                            // let subscriber connect to publisher
                            Thread.Sleep(500);

                            // server publishes some messages
                            var priceApple = new StockPrice() {When = DateTime.Now, Price = _price += 0.1M, Currency = "GBP", Symbol = "APPL"};
                            publisher.SendT(priceApple);

                            // all clients receives price

                            var quote1 = client1.ReceiveT<StockPrice>();
                            var quote2 = client2.ReceiveT<StockPrice>();

                            quote1.ShouldBeEquivalentTo(priceApple);
                            quote2.ShouldBeEquivalentTo(priceApple);
                            Console.WriteLine("recieved stock quote1:{0}", quote1);
                            Console.WriteLine("recieved stock quote2:{0}", quote2);
                        }
                    }
                }
            }
        }

        public class StockPrice
        {
            public string Symbol { get; set; }
            public DateTime When { get; set; }
            public decimal Price { get; set; }
            public string Currency { get; set; }
            public override string ToString()
            {
                return string.Format("{0}-{1}-{2}", When, Price, Currency);
            }
        }



    }
}
