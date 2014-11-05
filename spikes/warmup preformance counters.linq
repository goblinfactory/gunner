<Query Kind="Program">
  <Namespace>System.Net</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{		
		var urls = new [] {
		"http://localhost:8082/small1.json?buster=6a85a94a-3aa1-46bd-8e81-4e7e5992a6d1?buster=659d5019-0a31-4d31-8224-e354207165eb",
		"http://localhost:8082/small1.json?buster=6a85a94a-3aa1-46bd-8e81-4e7e5992a6d1?buster=23f95431-1848-4ff6-b8d7-5a747ee3a7cb",
		"http://localhost:8082/small1.json?buster=6a85a94a-3aa1-46bd-8e81-4e7e5992a6d1?buster=bfb553ac-f227-468f-95e9-4f8c9d812371",
		"http://localhost:8082/small1.json?buster=6a85a94a-3aa1-46bd-8e81-4e7e5992a6d1?buster=7a6e9347-35df-4608-99a4-9284f5077e09",
		"http://localhost:8082/small1.json?buster=6a85a94a-3aa1-46bd-8e81-4e7e5992a6d1?buster=48b0176b-5a3b-453b-9515-09dff1af1048",
		"http://localhost:8082/small1.json?buster=6a85a94a-3aa1-46bd-8e81-4e7e5992a6d1?buster=1a4b2e23-b9ec-47ad-a558-962e63f76aaf",
		"http://localhost:8082/small1.json?buster=6a85a94a-3aa1-46bd-8e81-4e7e5992a6d1?buster=4310c145-1bd9-4c49-8ac0-4958f6c88df5",
		"http://localhost:8082/small1.json?buster=6a85a94a-3aa1-46bd-8e81-4e7e5992a6d1?buster=6895e1fb-7cd9-4b21-8052-616ea29a3262",
		"http://localhost:8082/small1.json?buster=6a85a94a-3aa1-46bd-8e81-4e7e5992a6d1?buster=f6e9d7ad-8cd1-41fe-b737-c3758bd8dbf3"
		};
		
		QuickFire(urls,20);
		var readTask = Task.Run(()=> ReadCounters());
		var fireTask = Task.Run(()=> QuickFire(urls,250));
		Task.WaitAll(readTask,fireTask);
}

private void ReadCounters(){
		var pc = new PerformanceCounter();
		pc.ReadOnly = true;
		pc.CategoryName = @"W3SVC_W3WP";
		pc.InstanceName = @"_Total";
		pc.CounterName = @"Requests / Sec";
		for(int i=0;i<20;i++)
		{
			pc.NextValue().Dump();		
			pc.InstanceLifetime.Dump();
			Thread.Sleep(250);
		}			
}

private void QuickFire(string[] urls, int pause)
{
		using (var client = new WebClient())
			{
				for (int i = 0; i < 25; i++)
				{
					var url = urls[i % urls.Length];
					var result = client.DownloadString(url); Console.Write(".");
					Thread.Sleep(pause);
				}
			}
		Thread.Sleep(3000);	
}
