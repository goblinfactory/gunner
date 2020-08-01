Gunner
====

Gunner  command line Load test tool to rapid fire requests against a test or pre-production server. 
A design goal from the outset was to have as small a memory and cpu usage footprint as possible and at the same time be capable of delivering high concurrency load testing, basically it's a *super simplified poor man's load runner, and-or a one liner command-line jmeter replacement*.

Update : 1.8.2020
I wrote this code originally in 2014 thinking I could make a small commercial testing library that could use business'es developer machines after hours to run load tests in a small mesh or cluster, I forgot about it for a few years and now need to do some load testing of my own on some azure functions. My thinking is to take this console app, and publish it as a nuget package with a simpler API so that I can reference this from Linqpad and use this as a very simple framework for doing simple load tests that can rapidly tell me the breaking points (limits) of any code. (in my case, Azure functions).

Compatibility:
---
- .NET Framework 4.0+
- Untested on Mono.  

Current Release:
---
- Status is beta.

At a glance:
---
- xcopy deployable.
- Lots of similarities to curl and 
- One line server load testing.
- Low memory footprint (500 users at only 40Mb memory footprint.)
- [High throughput ( tested > 7000 rps)]
- High concurrency ( needs data. This was initially one of the drivers for writing gunner, to test concurrency, however throughput, requests per second, has since taken centre stage wrt priority. )
- Easy to use, get up and running and testing, in seconds.
- Use as a simple diagnostic tool to quickly identify network bottlenecks and test NFR's. (needs example)
- Envisage usage could be to keep a server idling warm or hot while journey testing is done using Selenium, Watin, Jmeter or other test tool, or manual testing.
- For testing simple API's : currently only supports GET, and no authentication. (Oauth in backlog)
- No support for cookies. (on backlog to add)
- Design goal is to be extendable to be able to be used to load test "user journeys", via Linqpad, as a fully fledged scripting IDE with intellisense and nuget support allowing you to write very simple journey scripts leveraging the power of RestSharp for full support to easily POST/PUT/GET/ADD complex entities to restful endpoints, oauth, basic authentication etc. (requires linqpad demo.)

Quickstart
----------

1.	Download zip, extract to any folder, optionally add the path to gunner to your env path.
1.  [start python webserver](https://stackoverflow.com/questions/17351016/set-up-python-simplehttpserver-on-windows) in the Gunner.www folder for a simple baseline test. `python -m http.server 8082`
1.	run gunner.

``gunner -start 10 -end 100 -repeat 100 -increment 10 -find "latitude" -root http://localhost:8082 -urls "small1.json,small2.json"`` 

The example above will run the following load against the server at ``http://localhost:8082`` 

- a ramping test, in batches from ``10`` to ``100`` concurrent users in steps of ``10``.
- Each simulated user will make 100 requests 
- Requests are split evenly between supplied urls, in the example above: ``small1.json`` and ``small2.json`` respectively.
- Tests will wait between each batch of users, for all users to finish and then pause.
- Default pause is ``5`` seconds between tests to allow the system to Idle. (can be overridden with -p setting.)

Here's the result of me testing 2000 to 5000 simultaneous users, running on a windows virtual machine, on a macpro.

	D:\gunner>gunner -u small1.json,small2.json -n latitude -s 1000 -e 5000 -i 1000 -r 10 -w http://localhost:8082/
	Gunner v 0.1
	date----------------,----total,--------rps,--users,success,---fail,--response
	2014-10-27 00:31:54Z,    10000,    8920.61,   1000,  10000,      0,  0.1121ms
	2014-10-27 00:32:01Z,    30000,    9451.80,   2000,  20000,      0,  0.1058ms
	2014-10-27 00:32:09Z,    60000,    9816.75,   3000,  30000,      0,  0.1019ms
	2014-10-27 00:32:18Z,   100000,    9878.98,   4000,  40000,      0,  0.1012ms
	Total requests:100000
	-------- finished ---------
 
Gunner runs and outputs the following:

- Sortable date
- Total requests that hit the server
- Ave requests per second, averaged over each batch
- Total count of success
- Total count of fails
- Average response time (ms) per request. TTLB (time to last byte)
 - This includes network latency so when testing server should be done within the same vlan as close to the server as possible, unless you're deliberately testing further away, in which case the difference between the different tests is very valuable.
- network traffic in megabytes recieved, on first ethernet card.
- network traffic in megabytes sent, on first ethernet card.
- Total ram that the journey scripts are using. Watch this figure when testing against large no of users.
Notes and known issues:
---
__**Gunner currently reports different values to what system performance monitors report, specifically the requests per second.**__ This may or may not be correct, despite the apparent contradiction. I am busy investigating further. I am manually testing by running the following on the command line of the windows server I'm hitting;


	typeperf "\W3SVC_W3WP(_Total)\Requests / Sec"
	this outputs ... 
	"10/27/2014 00:34:02.522","0.000000"
	"10/27/2014 00:34:03.524","0.000000"
	"10/27/2014 00:34:04.526","1996.073524"
	"10/27/2014 00:34:05.528","0.000000"
	"10/27/2014 00:34:06.530","0.000000"
	"10/27/2014 00:34:07.532","0.000000"
	"10/27/2014 00:34:08.534","0.000000"
	"10/27/2014 00:34:09.536","2994.225337"

# Gunner options

Run `gunner.exe` without passing in any options, or any invalid option parameters and you will get this output

```log

Gunner 0.1.1.43173
Gunner

  -a, --csv            relative path to a csv file containing urls to request.

  -b, --logerrors      (Default: False) Whether to log errors or not. If you
                       have errors, to reproduce just do a single run with 1
                       user afterwards with repeat set to = max urls in your
                       csv file.

  --errorfile          (Default: ) Error logfile to write out individual errors
                       to. Useful if you want to run a single test batch to
                       check entities before running a load test. (sanity check
                       first.) Differential between system performance issues,
                       and application or user errors. i.e. should not run a
                       load test if a sanity check test fails.

  -c, --cachebuster    (Default: False) Whether to append a cachebuster string
                       to the end of your url or not.

  -d, --delimiter      (Default: ,) Character delimiter for lists. If you want
                       to use a | then you will need to enclose any values
                       containing a pipe with quotes (")

  --sequential         (Default: False) Sequential? (false = random)

  -e, --end            (Default: 500) Total number of simultaneous user
                       connections (parallel connections) that the tests will
                       attempt to ramp up to.

  -f, --format         (Default: {0:u},{1,9},{2,11:0.00},{3,7},{4,7},{5,7},
                       {6,7:0.0000}ms,{7,7:0.00}Mb, {8,7:0.00}Mb, ({9,7:0.0}Mb
                       ram)) Test results format string. Please keep the
                       sequence the same, otherwise headers wont match and,or,
                       automated log reporting can fail.

  -g, --gap            (Default: 100) Pause (gap) between each request. (random
                       from 0 to this value) to avoid clumping.

  -h, --header         (Default:
                       date----------------,----total,--------rps,--users,succes
                       s,---fail,--response,--MB
                       (in),---MB(out),---------MB(RAM)) Default header for
                       formatting the console output.

  -i, --increment      (Default: 50) Number of concurrent users to increase
                       each test by.

  -l, --logfile        (Default: ) Name of logfile, optional relative path.
                       Will use path of local execution. Will overwrite file if
                       already exists, will create if not.

  -n, --find           Required. String to search for that must be returned in
                       the body of the response, that confirms the request was
                       valid.

  -p, --pause          (Default: 5000) Pause in ms between tests, allows a
                       webserver to settle and-or idle, also allows you to more
                       easily see the diferent test steps.

  -r, --repeat         (Default: 100) Number of times each user will request
                       the url given to him.

  -s, --start          (Default: 50) Number of concurrent users to start with.
                       Gunner will start with this value (s), run tests up
                       until (u), incrementing in steps of (i)

  --stagger            (Default: 100) Starting of the batches will be staggered
                       randomly between 0 and this value (ms) to avoid clumping.

  -t, --timeout        (Default: 600) Test timeout. Maximum time (in seconds)
                       allowed for each (up to the final batch) to all return.
                       (not yet done any decent reporting for when this
                       happens. TODO)

  -u, --urls           delimited list of urls to test.  Each user will be
                       assigned a url from the list of urls, using round robin.

  -v, --verbose        (Default: False) displays response of first 10 requests
                       in console. (useful for confirming network connectivity.)

  -w, --root           (Default: http://localhost/) root website prefix for all
                       the urls,so that the urls can be smaller and easier to
                       write in a list

  --skipbatch          (Default: 0) Number of batches to skip from logging,
                       reporting and testing. So that the system can be warmed
                       up before monitoring acts on the results. (Normally you
                       need to skip enough batches that your monitoring
                       averages don't overlap a period while your testing had
                       not yet started.)

  --warningshot        (Default: False) Fire off 1 random request (warning
                       shot) to wake up the server before starting any testing.
                       Useful if you want to wake up a server with a single
                       request.

  --master             (Default: 127.0.0.1) ip address of the server that
                       gunner is running in master mode on.

  --port               (Default: 9090) port that gunner is listening on. (this
                       is the same port on all gunner masters or slaves.)

  --mode               (Default: ) master | slave | standalone (leave null, do
                       not supply if running standalone.)

  --help               Display this help screen.

```

Acknowledgements:
---
Thanks to [Giacomo Stelluti](https://github.com/gsscoder) Scala for the excellent [CommandLineParser](https://github.com/gsscoder/commandline/wiki/Quickstart), that has made the help text and command parsing, a critical aspect of any commandline tool a pleasure to implement.


Requirements
---
* .Net 4.5

  Gunner was built with .net 4.5. It's in the backlog to consider a lesser .net version so that there's less need for .net updates on client (test) servers.


Immediate Current work
---
 - Get tests results as accurate as is needed to be a useful tool. Possibly write an acceptance test, something along the lines of:
 - show total network traffic. 
 - autodetect network card.
```
	Given gunner 
	And a webserver
	When gunner is running 
	And reporting requests per second of (x)
	Then the webserver should report similar requests per second within an acceptable tolerance  
```

 - CSV input of urls. If any of the urls passed is a file, then url values will be read from the file. Two values per line -> url, find
 
Todo
---

1. Update this readme. As of 16/11/14, this is a bit out of date. Oops!
1. Install script that checks that IIS is available, and if yes, creates localhost:8082 website, so that unit tests for performance will be able to be run.

* Roadmap (high priority ideas)
---

 - Put in place decent test coverage, and CI server so that contributors can easily contribute. With decent tests in place, could then easily port to Java and other languages.
 - Need to stagger requests, so that we don't have clumping when users all start requesting at once. (can reduce impact of this by ensuring users get random files.)


Idea backlog (unsorted)
---
  - __gzip__, gzip support? Add and test. ``client.Headers["Accept-Encoding"] = "gzip";``
  - __auto detect nic__, so that we can automatically tell when testing using wifi, localhost or multiple adapters. (autodetecting seems plausable, sort by NIC with highest traffic.)
  - __random seed__, so that random tests can be re-run with the same requests and timings. In case of unexpected clumping.
  - __extend csv__, add extra fields to csv file, (url,verb,status,find,body)
  - __authentication__ oauth, forms, and custom headers, so that can be used for journey testing.
  - __cookies,other verbs, test state keyval store, body post__, so that can easily be used for journey testing.
  - __nlog__ (would allow for logging to be configured and output to Elastic Search, which would allow using ELK to investigate bigger data sets, e.g. when doing a root cause analysis.)
  - __Linqpad version__ (i.e. make it available as a linqpad script.) So that it can be used in more high security scenarios where all software and tools used on servers have to be reviewed by an appropriate admin. 
  - __package as a single exe__, so that you more easily copy and run on a machine.
  - __run as a windows service__, so that it can be resiliant, with auto resume on failure, and be monitored centrally. Make use of operating system built in watchdog facilities.
  - __master and slave install__, so that you can run tests from multiple machines.
  - __alerting__, so that alerts can be triggered if deviation exceeds configured tolerance.
  - __Ruby and, or Java version__, so that Gunner can be quickly and easily installed on non windows server. 
  - __Compute hash and publish for the packages__, so that users can confirm their copy is not corrupted.
  - Miscanlaneous : keepalive, http pipelining,measure bytes transferred. 
  - Final thoughts
   - Would be nice to be able to remotely query a server and read some key perfomance values, e.g. shell to typeperf or similar, read network utilisation, __so that we can start to easily see whether it's a network bottleneck vs us reaching server capacity.__ (i.e. is there a really quick win here, that could give us 90% of what we often need, at 1% of the cost in time and effort.)
 

Contacts
---
Alan Hemmings

  - goblinfactory AT gmail DOT com
  - [About.me](http://about.me/alanhemmings)
  - [www.goblinfactory.co.uk](http://www.goblinfactory.co.uk)
  - [snowcode.com - developer conferences at ski resorts](http://www.snowcode.com)

  * this was the roadmap in 2014, I've checked this in with minimal changes.