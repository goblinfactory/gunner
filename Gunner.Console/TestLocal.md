# TestLocal explained

	Gunner  -l "mock-counter.log" -g 0 -s 10 -e 100 -r 100 -p 1000 -i 10 -n "latitude" -w "http://localhost:8082/" -u "small1.json,large1.json,small2.json,large2.json,small3.json,large3.json,small4.json,large4.json"

Gunner  

#### --logfile

> -l "mock-counter.log" 

- Name of logfile, optional relative path.
- Will use path of local execution. Will overwrite file if already exists, will create if not.	

#### --gap

> -g 0 

- Pause (gap) between each request. 
- (random from 0 to this value) to avoid clumping.
- (Default: 100)

#### --start

> -s 10

- Number of concurrent users to start with.
- Gunner will start with this value (s), run tests up until (u), incrementing in steps of (i)
- (Default: 50)

#### --end

> -e 100 

- Total number of simultaneous user connections (parallel connections) that the tests will attempt to ramp up to.  
- (Default: 500)

#### --repeat

> -r 100 

- Number of times each user will request the url given to him.
- (Default: 100)

#### --pause

> -p 1000 

- Pause in ms between tests, allows a webserver to settle and-or idle, also allows you to more easily see the diferent test steps.
- (Default: 5000)

#### --increment

> -i 10 

- Number of concurrent users to increase each test by.
- (Default: 50)

#### --find

> -n "latitude" 

- String to search for that must be returned in the body of the response, that confirms the request was valid.
- Required.

#### --root

> -w "http://localhost:8082/" 

- root website prefix for all the urls,so that the urls can be smaller and easier to write in a list
- (Default: http://localhost/)

#### --urls

> -u "small1.json,large1.json,small2.json,large2.json,small3.json,large3.json,small4.json,large4.json"

- delimited list of urls to test.  
- Each user will be assigned a url from the list of urls, using round robin.


