using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public class Downloader : IDownloader
    {
        private readonly List<string> _errors;
        private DateTime _lastFlush;

        public Downloader(List<string> errors, ref DateTime lastFlush)
        {
            _errors = errors;
            _lastFlush = lastFlush;
        }

        private void LogError(string url, Exception ex, string path)
        {
            // NB! this will (may?) cause problems at high concurrency!
            string error = string.Format("ERROR {0} : {1} {2}", url, ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
            _errors.Add(error);
            if (DateTime.Now.Subtract(_lastFlush).TotalSeconds > 5)
            {
                _lastFlush = DateTime.Now;
                File.AppendAllLines(path, _errors.ToArray());
                _errors.Clear();
            }
        }

        public DownloadResult Download(string url, WebClient client, string find, bool verbose, int verboseMessagesToShow, bool cachebust, string logPath, bool logErrors)
        {
            var dr = new DownloadResult();
            try
            {
                var result = client.DownloadString(url);
                if (result.Contains(find))
                {
                    dr.Success = true;
                    return dr;
                }
                dr.Success = false;
                return dr;
            }
            catch (WebException we)
            {
                if (logErrors) LogError(url, we, logPath);
                dr.Success = false;

                if (we.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = we.Response as HttpWebResponse;
                    if (response != null)
                    {
                        dr.ErrorCode = (int)response.StatusCode;
                    }
                }
                if (logErrors) LogError(url, we, logPath);
                return dr;
            }
            catch (Exception ex)
            {
                if (logErrors) LogError(url, ex, logPath);
                dr.Success = false;
                return dr;
            }

        }
    }
}
