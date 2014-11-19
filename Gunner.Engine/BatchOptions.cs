using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Gunner.Engine
{
    public class BatchOptions : IUrlReader, IReporterFormat
    {
        public BatchOptions()
        {
            // some useful defaults for testing
            Header = BatchOptions._defaultHeader;
            Format = BatchOptions._defaultFormat;
            Delimiter = ",";
            LogErrors = false;
            Timeout = 300;
            StaggerStart = 100;
        }

        [Option('a', "csv",
            HelpText = "relative path to a csv file containing urls to request.")]
        public string Csv { get; set; }

        [Option('b', "logerrors", DefaultValue = false,
            HelpText = "Whether to log errors or not. If you have errors, to reproduce just do a single run with 1 user afterwards with repeat set to = max urls in your csv file.")]
        public bool LogErrors { get; set; }

        [Option( "errorfile", DefaultValue = "",
            HelpText = "Error logfile to write out individual errors to. Useful if you want to run a single test batch to check entities before running a load test. (sanity check first.) Differential between system performance issues, and application or user errors. i.e. should not run a load test if a sanity check test fails.")]
        public string ErrorLogfile { get; set; }


        [Option('c', "cachebuster", DefaultValue = false,
            HelpText = "Whether to append a cachebuster string to the end of your url or not.")]
        public bool Cachebuster { get; set; }

        [Option('d', "delimiter", DefaultValue = ",",
           HelpText = "Character delimiter for lists. If you want to use a | then you will need to enclose any values containing a pipe with quotes (\")")]
        public string Delimiter { get; set; }

        
        [Option('e', "end", DefaultValue = 500,
            HelpText = "Total number of simultaneous user connections (parallel connections) that the tests will attempt to ramp up to.")]
        public int End { get; set; }

        public const string _defaultFormat = "{0:u},{1,9},{2,11:0.00},{3,7},{4,7},{5,7}, {6,7:0.0000}ms,{7,7:0.00}Mb, {8,7:0.00}Mb, ({9,7:0.0}Mb ram)";

        [Option('f', "format", Required = false, DefaultValue = _defaultFormat,
            HelpText = "Test results format string. Please keep the sequence the same, otherwise headers wont match and,or, automated log reporting can fail.")]
        public string Format { get; set; }

        [Option('g', "gap", DefaultValue = 100,
    HelpText = "Pause (gap) between each request. (random from 0 to this value) to avoid clumping.")]
        public int Gap { get; set; }

        public const string _defaultHeader = "date----------------,----total,--------rps,--users,success,---fail,--response,--MB (in),---MB(out),---------MB(RAM)";
                                            //"2014-10-26 20:55:19Z,       50,    1351.35,     50,      0,     50,  0.7400ms,   0.00Mb,    0.00Mb, (   20.7Mb ram)
        [Option('h', "header", DefaultValue = BatchOptions._defaultHeader,
        HelpText = "Default header for formatting the console output.")]
        public string Header { get; set; }


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

        [Option("stagger", DefaultValue = 100,
            HelpText = "Starting of the batches will be staggered randomly between 0 and this value (ms) to avoid clumping.")]
        public int StaggerStart { get; set; }


        [Option('t', "timeout", DefaultValue = 600,
            HelpText = "Test timeout. Maximum time (in seconds) allowed for each (up to the final batch) to all return. (not yet done any decent reporting for when this happens. TODO)")]
        public int Timeout { get; set; }

        [Option('u', "urls", Required = false,
           HelpText = "delimited list of urls to test.  Each user will be assigned a url from the list of urls, using round robin.")]
        public string UrlList { get; set; }

        [Option('v', "verbose", DefaultValue = false,
   HelpText = "displays response of first 10 requests in console. (useful for confirming network connectivity.)")]
        public bool Verbose { get; set; }

        [Option('w', "root", Required = false, DefaultValue = "http://localhost/",
            HelpText = "root website prefix for all the urls,so that the urls can be smaller and easier to write in a list")]
        public string Root { get; set; }

        [Option( "skipbatch", Required = false, DefaultValue = 0,
            HelpText = "Number of batches to skip from logging, reporting and testing. So that the system can be warmed up before monitoring acts on the results. (Normally you need to skip enough batches that your monitoring averages don't overlap a period while your testing had not yet started.)")]
        public int SkipBatches { get; set; }

        [Option("warningshot", Required = false, DefaultValue = false,
            HelpText = "Fire off 1 random request (warning shot) to wake up the server before starting any testing. Useful if you want to wake up a server with a single request.")]
        public bool WarningShot { get; set; }

        [Option("master", DefaultValue = "127.0.0.1",
        HelpText = "ip address of the server that gunner is running in master mode on.")]
        public string Master { get; set; }

        [Option("port", DefaultValue = 9090,
        HelpText = "port that gunner is listening on. (this is the same port on all gunner masters or slaves.)")]
        public int Port { get; set; }

        [Option("mode", Required = false, DefaultValue = null,
        HelpText = "master | slave | standalone (leave null, do not supply if running standalone.)")]
        public string Mode { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,(HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}