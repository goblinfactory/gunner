Performance notes:
====

The actual throughput (rps:requests per second) that you will get from Gunner will depending primarily on four things

1. The system you're testing.
1. Your test machine hardware.
1. The network topology between gunner and the system you want to put under "load".
1. The size (in bytes) of the responses.

To help you get a feel for how gunner works, if you have checked out the project source, inside the folder ``Gunner.www`` ,are 8 sample json files, 4 small (6 Kb), and 4 large (716 Kb). Below is the output of two tests, showing the difference in (RPS) when processing small and large responses respectively.

Size of response - impact on performance
---
_Below are tests results on the same machine, run immediately after each other, where the only difference between the two tests was the size of the response file. ``u=small1.json vs u=large1.json``_ 


*small response file (6Kb) = > __7800 rps__* 

	D:\gunner>gunner --start 3 --end 5 -i 1 -p 0 -r 100 -w http://localhost:8082/ -n latitude -g 0 -u small1.json
	Gunner v 0.1
	date----------------,----total,--------rps,--users,success,---fail,--response,--------Mb used
	2014-10-30 08:11:37Z,      300,    2222.22,      3,    300,      0,  0.4500ms (   11.3Mb used)
	2014-10-30 08:11:38Z,      700,    5479.45,      4,    400,      0,  0.1825ms (   11.6Mb used)
	2014-10-30 08:11:38Z,     1200,    7812.50,      5,    500,      0,  0.1280ms (   11.8Mb used)
	Total requests:1200
	-------- finished ---------

*large response file (716Kb) = < __180 rps__* 

	D:\gunner>gunner --start 3 --end 5 -i 1 -p 0 -r 100 -w http://localhost:8082/ -n latitude -g 0 -u large1.json
	Gunner v 0.1
	date----------------,----total,--------rps,--users,success,---fail,--response,--------Mb used
	2014-10-30 08:14:38Z,      300,     172.41,      3,    300,      0,  5.8000ms (   23.0Mb used)
	2014-10-30 08:14:42Z,      700,     101.63,      4,    400,      0,  9.8400ms (   24.3Mb used)
	2014-10-30 08:14:47Z,     1200,      95.57,      5,    500,      0, 10.4640ms (   50.4Mb used)
	Total requests:1200
	-------- finished ---------

Simulating network latency with -g (gap)
---
The gap setting, allows you to specify a pause in ms between each user request. Tests run in parallel
(TODO: need to stagger the requests, otherwise we will have clumping of user requests.)