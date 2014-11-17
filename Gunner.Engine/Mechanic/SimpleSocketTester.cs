using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace Gunner.Engine.Mechanic
{
    public class RequestReply
    {
        public string Request { get; set; }
        public string Reply { get; set; }
    }

    public class SimpleSocketTester : IDisposable
    {
        private int _port;
        private readonly ILogWriter[] _writers;
        private string _bind;
        private NetMQContext _ctx;

        public SimpleSocketTester(string server, int port) : this(server,port, new[] { (ILogWriter)new ConsoleWriter()}) {}

        public SimpleSocketTester(string server, int port, ILogWriter[] writers)
        {
            _writers = writers;
            _bind = string.Format("tcp://{0}:{1}", server, port);
            _ctx = NetMQContext.Create();
            _port = port;
        }

        //TODO: configure the sent and recieved formats
        public RequestReply[] SendMessages(int pauseMsBetweenMessages, params string[] messages)
        {
            var messageResponses = new List<RequestReply>();
            using (var socket = _ctx.CreateRequestSocket())
            {

                socket.Connect(_bind);
                _writers.WriteLine("Opening connection to :{0}", _bind);
                foreach(var message in messages)
                {
                    var rr = new RequestReply();
                    _writers.Write("-> {0}",message);
                    socket.Send(message);
                    rr.Request = message;
                    var response = socket.ReceiveString();
                    rr.Reply = response;
                    messageResponses.Add(rr);
                    _writers.WriteLine(" -> {0}",response);
                    Thread.Sleep(pauseMsBetweenMessages);
                }
                _writers.WriteLine("closing connection.");
            }
            return messageResponses.ToArray();
        }

        public RequestReply[] RecieveMessage(string viewresponse, int cnt)
        {
            var messages = new List<RequestReply>();
            using (var socket = _ctx.CreateResponseSocket())
            {
                socket.Bind(_bind);
                _writers.WriteLine("Listening on port {0} for {1} messages", _port,cnt);
                for (int i = 0; i < cnt; i++)
                {
                    var rr = new RequestReply();
                    var message = socket.ReceiveString();
                    rr.Request = message;
                    _writers.Write("-> {0}", message);
                    var response = FormatRecieveResponse(viewresponse, message);
                    rr.Reply = response;
                    _writers.WriteLine(" -> {0}", response);
                    socket.Send(response);
                    messages.Add(rr);
                }
                _writers.WriteLine("listener closing connection.");
                return messages.ToArray();
            }

        }

        public string FormatRecieveResponse(string response, string message)
        {
            var formatted = response.Replace("{message}", message);
            return formatted;
        }

        bool _disposed;

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SimpleSocketTester()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) { _ctx.Dispose(); }
            _disposed = true;
        }

    }

}
