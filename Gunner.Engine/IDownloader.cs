using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public interface IDownloader
    {
        DownloadResult Download(string url, WebClient client, string find, bool verbose, int verboseMessagesToShow, bool cachebust, string logPath, bool logErrors);
    }
}
