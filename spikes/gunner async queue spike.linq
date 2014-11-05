<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.Parallel.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Timer.dll</Reference>
  <Namespace>System</Namespace>
  <Namespace>System.Collections.Generic</Namespace>
  <Namespace>System.Linq</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text</Namespace>
  <Namespace>System.Threading</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{

	// NB! also take a look at this great example:
	// http://stackoverflow.com/questions/19431494/how-to-use-await-in-a-loop
	var r = new Runner();
	r.Run(20);
	Console.WriteLine("press enter to finish.");
	Console.ReadLine();
	r.Cancel();
}

    public class Runner
    {
        CancellationTokenSource cts;

        public async void Run(int repeat)
        {
			Console.WriteLine("running");
            cts = new CancellationTokenSource();

            try
            {
				// create (n) tasks, then awaitany until done, repeat the inner process?
                await RunClientScript(cts.Token,repeat);
                Console.WriteLine("Downloads complete.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Downloads canceled.");
            }
            catch (Exception)
            {
                Console.WriteLine("Downloads failed.");
            }

            cts = null;
        }


        public void Cancel()
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }


        async Task RunClientScript(CancellationToken ct, int requests)
        {
            HttpClient client = new HttpClient();
			
			IEnumerable<Task<int>> downloadTasksQuery =Enumerable
				.Range(1,requests)
				.Select(i => ProcessURL(RandomUrl(), client, ct));
				
			List<Task<int>> downloadTasks = downloadTasksQuery.ToList();
			int cnt = 0;
            while (downloadTasks.Count > 0)
            {
                    Task<int> firstFinishedTask = await Task.WhenAny(downloadTasks);
                    downloadTasks.Remove(firstFinishedTask);
                    int length = await firstFinishedTask;		
					cnt++;
            }
			Console.WriteLine("Requests:  {0}", cnt);
        }

		private List<string> urls = new List<string> 
            { 
				"http://localhost:8082/large1.json",
				"http://localhost:8082/large2.json",
				"http://localhost:8082/large3.json",
				"http://localhost:8082/large4.json",
				"http://localhost:8082/small1.json",
				"http://localhost:8082/small2.json",
				"http://localhost:8082/small3.json",
				"http://localhost:8082/small4.json"
            };

        private string RandomUrl()
        {
            int len = urls.Count();
			return urls[new Random().Next(len-1)];			
        }


        async Task<int> ProcessURL(string url, HttpClient client, CancellationToken ct)
        {
			// return http status code
            HttpResponseMessage response = await client.GetAsync(url, ct);
            byte[] urlContents = await response.Content.ReadAsByteArrayAsync();
            return urlContents.Length;
        }
    }