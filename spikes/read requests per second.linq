<Query Kind="Statements" />

//var pc = new PerformanceCounter(@"\W3SVC_W3WP(_Total)\Requests / Sec");
using(var pc = new PerformanceCounter())
{
	//@"W3SVC_W3WP",@"(_Total)\Requests / Sec"
	
	pc.CategoryName = @"W3SVC_W3WP";
	pc.InstanceName = @"_Total";
	// how to read all the counters in this space?
	//pc.CounterName = @"Total Get Requests";
	pc.CounterName = @"Requests / Sec";
	

	
	for(int i=0;i<100;i++)
	{
		Console.WriteLine(pc.NextValue());
		Thread.Sleep(1000);
	}
	
}