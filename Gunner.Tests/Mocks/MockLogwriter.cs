using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gunner.Engine;

namespace Gunner.Tests.Mocks
{
    public class MockLogWriter : ILogWriter
    {
        private string partLine = "";
        private readonly bool _echoConsole;
        private readonly List<string> _lines = new List<string>(); 
        
        public MockLogWriter(bool echoConsole)
        {
            _echoConsole = echoConsole;
        }

        public void WriteLine(string line)
        {
            var fullLine = string.Format("{0}{1}", partLine, line);
            _lines.Add(fullLine);
            if (_echoConsole) Console.WriteLine(fullLine);
            partLine = "";

        }

        public void Write(string text)
        {
            partLine = text;
        }

        public List<string> ReadLines()
        {
            var lines = _lines.ToList();
            if (! string.IsNullOrWhiteSpace(partLine)) lines.Add(partLine);
            return lines;
        } 
    }
}
