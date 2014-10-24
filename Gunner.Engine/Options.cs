using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Gunner.Engine
{
    public class Options
    {

        [Option('f', "format", Required = false, DefaultValue = "{0,-7},{1,-11:0.00},{2,-7},{3,-7},{4,-7}, {5,-7:0.0000}ms",
            HelpText = "Test results format string. Please keep the sequence the same, otherwise headers wont match and,or, automated log reporting can fail.")]
        public string Format { get; set; }

        [Option('v', "verbose", DefaultValue = false,
   HelpText = "displays response of first 10 requests in console. (useful for confirming network connectivity.)")]
        public bool Verbose { get; set; }

        [Option('l', "logfile", DefaultValue = "",
           HelpText = "Name of logfile, optional relative path. Will use path of local execution. Will overwrite file if already exists, will create if not.")]
        public string Logfile { get; set; }

        public string LogPath { get; set; }

        [Option('u', "urls", Required = true,
           HelpText = "delimited list of urls to test.  Each user will be assigned a url from the list of urls, using round robin.")]
        public string UrlList { get; set; }

        public string[] Urls
        {
            get { return UrlList.Split(new string[] {Delimiter}, StringSplitOptions.RemoveEmptyEntries); }
        }

        [Option('d', "delimiter", DefaultValue = ",",
           HelpText = "Character delimiter for lists. If you want to use a | then you will need to enclose any values containing a pipe with quotes (\")")]
        public string Delimiter { get; set; }
        

        [Option('t', "total", DefaultValue = 500,
            HelpText = "Total number of simultaneous user connections to attempt in parallel.")]
        public int Users { get; set; }

        [Option('m', "timeout", DefaultValue = 600,
            HelpText = "Test timeout. Maximum time (in seconds) allowed for each (up to the final batch) to all return. (not yet done any decent reporting for when this happens. TODO)")]
        public int Timeout { get; set; }


        [Option('p', "pause", DefaultValue = 5000,
            HelpText = "Pause in ms between tests, allows a webserver to settle and-or idle, also allows you to more easily see the diferent test steps.")]
        public int Pause { get; set; }

        [Option('r', "repeat", DefaultValue = 100,
            HelpText = "Number of times each user will repeat the test.")]
        public int Repeat { get; set; }

        [Option('s', "start", DefaultValue = 500,
            HelpText = "Number of concurrent users to start with. Gunner will start with this value (s), run tests up until (u), incrementing in steps of (i) ")]
        public int Start { get; set; }

        [Option('i', "increment", DefaultValue = 50,
            HelpText = "Number of concurrent users to increase each test by.")]
        public int Increment { get; set; }

        [Option('c', "cachebuster", DefaultValue = true,
            HelpText = "Whether to append a cachebuster string to the end of your url or not.")]
        public bool Cachebuster { get; set; }

        [Option('n', "find", Required = true,
            HelpText = "String to search for that must be returned in the body of the response, that confirms the request was valid.")]
        public string Find { get; set; }


        //[Option('a', "action", DefaultValue = "GET",
        //   HelpText = "Prints all messages to standard output.")]
        //public bool Action { get; set; }

        //[Option('b', "body", DefaultValue = true,
        //   HelpText = "Body of message to PUT,POST,DELETE,ADD etc")]
        //public bool Body { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                                      (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}