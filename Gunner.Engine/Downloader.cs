using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public interface IErrorLoggerSettings
    {
        string LogFile { get; set; }
   
    }

    public interface IErrorLogger
    {
        void LogError(string url, Exception ex);
    }

    public class ErrorLogger : IErrorLogger
    {
        private readonly List<string> _errors;
        private DateTime _lastFlush;
        private readonly string _errorLogPath;
        private readonly bool _logErrors;

        public ErrorLogger(string errorLogPath, bool logErrors)
        {
            _errors = new List<string>();
            _lastFlush = DateTime.Now;
            _errorLogPath = errorLogPath;
            _logErrors = logErrors;
        }

        public void LogError(string url, Exception ex)
        {
            if (!_logErrors) return;
            // NB! this will (may?) cause problems at high concurrency!
            string error = string.Format("ERROR {0} : {1} {2}", url, ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
            _errors.Add(error);
            
            // NB! need a datetime provider!!?? ADH: not for now, this class is easy to mock and it encapsulates most of the usages
            // NB! of the filesystem and date time.
            if (DateTime.Now.Subtract(_lastFlush).TotalSeconds > 5)
            {
                _lastFlush = DateTime.Now;
                File.AppendAllLines(_errorLogPath, _errors.ToArray());
                _errors.Clear();
            }
        }

    }

    public class Downloader : IDownloader
    {
        private readonly IErrorLogger _errorLogger;

        public Downloader(IErrorLogger errorLogger)
        {
            _errorLogger = errorLogger;
        }

        public DownloadResult Download(string url, WebClient client, string find, bool cachebust)
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
                dr.Success = false;
                if (we.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = we.Response as HttpWebResponse;
                    if (response != null)
                    {
                        dr.ErrorCode = (int)response.StatusCode;
                    }
                }
                _errorLogger.LogError(url, we);
                return dr;
            }
            catch (Exception ex)
            {
                _errorLogger.LogError(url, ex);
                dr.Success = false;
                return dr;
            }

        }
    }
}
