using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace Gunner.Engine.Mechanic
{
//  > mechanic send -s 127.0.0.1 -p 9090 -m "hello"
//  > mechanic listen -p 9090 
    enum TestMode { Send, Receive }
    
    public class MechanicOptions
    {
        [VerbOption("listen",HelpText = "listen for test messages on tcp, useful for diagnosing network issues.")]
        public ListenSubOptions ListenVerb { get; set; }

        [VerbOption("send", HelpText = "Send test messages over tcp, useful for diagnosing network issues.")]
        public SendSubOptions SendVerb { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

    }

    public static class Verb
    {
        public const string listen = "listen";
        public const string send = "send";
    }


    public class ListenSubOptions
    {
        public ListenSubOptions()
        {
            Response = "{message} received.";
            Cnt = 1;
            Port = 9090;
        }
        [Option('r', "response", DefaultValue = "'{message}' received.",
        HelpText = "Response(s) to send to sender. Optionally include {message} to format a reply. Seperate multiple responses with |. Put quotes around if use a pipe in this value.")]
        public string Response { get; set; }

        [Option('s', "server", Required = true,
            HelpText = "server to send to.")]
        public string Server { get; set; }

        [Option('c', "cnt", DefaultValue = 1,
        HelpText = "Number of messages to wait for. Select 0 to run continuously.")]
        public int Cnt { get; set; }

        [Option('p', "port", DefaultValue = 9090,
        HelpText = "Port to send to server port.")]
        public int Port { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

    }


    public class SendSubOptions
    {
        public SendSubOptions()
        {
            Port = 9090;
            Wait = 1000;
            Message = "Test Message";
        }

        [Option('p', "port", DefaultValue = 9090,
        HelpText = "Port to send to server port.")]
        public int Port { get; set; }

        [Option('w', "wait", DefaultValue = 1000,
        HelpText = "ms to wait between messages.")]
        public int Wait { get; set; }


        [Option('s', "server", Required = true,
            HelpText = "server to send to.")]
        public string Server { get; set; }

        [Option('m', "message", Required = false, DefaultValue = "Test Message",
            HelpText = "Message text to send")]
        public string Message { get; set; }

        public string[] Messages
        {
            get { return Message.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries); }
        }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

    }


}
