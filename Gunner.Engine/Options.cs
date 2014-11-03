using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Gunner.Engine
{
    public class Options
    {
        [Option('a', "csv",
            HelpText = "relative path to a csv file containing urls to request.")]
        public string Csv { get; set; }

        [Option('b', "logerrors", DefaultValue = false,
            HelpText = "Whether to log errors or not. If you have errors, to reproduce just do a single run with 1 user afterwards with repeat set to = max urls in your csv file.")]
        public bool LogErrors { get; set; }


        [Option('c', "cachebuster", DefaultValue = false,
            HelpText = "Whether to append a cachebuster string to the end of your url or not.")]
        public bool Cachebuster { get; set; }

        [Option('d', "delimiter", DefaultValue = ",",
           HelpText = "Character delimiter for lists. If you want to use a | then you will need to enclose any values containing a pipe with quotes (\")")]
        public string Delimiter { get; set; }

        //TODO: create default formats for each of the columns, then let user select columns?
        public const string DefaultFormat = "{0:u},{1,9},{2,11:0.00},{3,7},{4,7},{5,7}, {6,7:0.0000}ms,{7,7:0.00}Mb, {8,7:0.00}Mb, ({9,7:0.0}Mb ram)";
                                          //"2014-10-26 20:55:19Z,       50,    1351.35,     50,      0,     50,  0.7400ms,   0.00Mb,    0.00Mb, (   20.7Mb ram)
        public const string DefaultHeader = "date----------------,----total,--------rps,--users,success,---fail,--response,--MB (in),---MB(out),---------MB(RAM)";

        [Option('e', "end", DefaultValue = 500,
            HelpText = "Total number of simultaneous user connections (parallel connections) that the tests will attempt to ramp up to.")]
        public int Users { get; set; }


        [Option('f', "format", Required = false, DefaultValue = Options.DefaultFormat,
            HelpText = "Test results format string. Please keep the sequence the same, otherwise headers wont match and,or, automated log reporting can fail.")]
        public string Format { get; set; }

        [Option('g', "gap", DefaultValue = 100,
    HelpText = "Pause (gap) between each request.")]
        public int Gap { get; set; }


        [Option('i', "increment", DefaultValue = 50,
            HelpText = "Number of concurrent users to increase each test by.")]
        public int Increment { get; set; }

        [Option('l', "logfile", DefaultValue = "",
           HelpText = "Name of logfile, optional relative path. Will use path of local execution. Will overwrite file if already exists, will create if not.")]
        public string Logfile { get; set; }

        //public string LogPath { get; set; }

        [Option('n', "find", Required = true,
            HelpText = "String to search for that must be returned in the body of the response, that confirms the request was valid.")]
        public string Find { get; set; }

        [Option('p', "pause", DefaultValue = 5000,
            HelpText = "Pause in ms between tests, allows a webserver to settle and-or idle, also allows you to more easily see the diferent test steps.")]
        public int Pause { get; set; }

        [Option('r', "repeat", DefaultValue = 100,
            HelpText = "Number of times each user will request the url given to him.")]
        public int Repeat { get; set; }

        [Option('s', "start", DefaultValue = 50,
            HelpText = "Number of concurrent users to start with. Gunner will start with this value (s), run tests up until (u), incrementing in steps of (i) ")]
        public int Start { get; set; }

        [Option('t', "timeout", DefaultValue = 600,
            HelpText = "Test timeout. Maximum time (in seconds) allowed for each (up to the final batch) to all return. (not yet done any decent reporting for when this happens. TODO)")]
        public int Timeout { get; set; }

        //public string[] Urls
        //{
        //    get { return UrlList.Split(new string[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries); }
        //}

        [Option('u', "urls", Required = false,
           HelpText = "delimited list of urls to test.  Each user will be assigned a url from the list of urls, using round robin.")]
        public string UrlList { get; set; }

        [Option('v', "verbose", DefaultValue = false,
   HelpText = "displays response of first 10 requests in console. (useful for confirming network connectivity.)")]
        public bool Verbose { get; set; }

        [Option('w', "root", Required = false, DefaultValue = "http://localhost/",
            HelpText = "root website prefix for all the urls,so that the urls can be smaller and easier to write in a list")]
        public string Root { get; set; }

        //[Option('a', "action", DefaultValue = "GET",
        //   HelpText = "Prints all messages to standard output.")]
        //public bool Action { get; set; }

        //[Option('b', "body", DefaultValue = true,
        //   HelpText = "Body of message to PUT,POST,DELETE,ADD etc")]
        //public bool Body { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,(HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}