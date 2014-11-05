using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Gunner.Engine;

namespace Gunner.Tests.Mocks
{
    public class MockDownloader : IDownloader
    {
        private readonly DownloadResult _result;

        public MockDownloader(DownloadResult result)
        {
            _result = result;
        }

        public DownloadResult Download(string url, WebClient client, string find, bool cachebust)
        {
            return _result;
        }
    }
}
