using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public class UrlReader
    {
        private string _delimiter;
        private string _uriRoot;
        private string _urls;
        private string _csv;
        private bool _cachebust;

        public UrlReader(Options option)
        {
            _delimiter = option.Delimiter;
            _urls = option.UrlList;
            _uriRoot = option.Root;
            _csv = option.Csv;
            _uriRoot = option.Root;
            _cachebust = option.Cachebuster;
        }

        public string[] ReadUrls(string currentDirectory)
        {
            if (_csv.HasText()) return ReadCsv(currentDirectory);
            if (_urls.HasText()) return SplitUrls();
            throw new ArgumentException("could not read urls, neither csv or urls has been set.");
        }

        private string Bust(string src)
        {
            string busted = _cachebust ? "?buster=" + Guid.NewGuid().ToString() : "";
            return string.Format("{0}{1}", src, busted);
        }
        
        private string[] SplitUrls()
        {
            var urlList = new string[] { };
            urlList = _urls
            .Split(new [] { _delimiter }, StringSplitOptions.RemoveEmptyEntries)
            .Select(u => string.Format("{0}{1}", _uriRoot, Bust(u)))
            .ToArray();
            return urlList;
        }


        private string[] ReadCsv(string currentDirectory){
            var urlList = new string[]{};
            string path = string.Format(@"{0}\{1}",currentDirectory, _csv);
            
            if (!File.Exists(path)) throw new FileNotFoundException("Could not find csv file:" + path);
            try
            {
                urlList = File
                    .ReadAllLines(path)
                    .Select(u=> u.Trim())
                    .Where(u=> !string.IsNullOrWhiteSpace(u))
                    .Select(u=> string.Format("{0}{1}",_uriRoot,Bust(u))).ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not parse url list from :" + path + ". Errror was:" + ex.Message);
            }
            return urlList;
        }
    }
}
