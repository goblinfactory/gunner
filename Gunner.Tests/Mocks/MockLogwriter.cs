using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gunner.Engine;

namespace Gunner.Tests.Mocks
{
    public class MockLogwriter : ILogWriter
    {
        private readonly bool _echoConsole;
        private readonly List<string> _lines = new List<string>(); 
        
        public MockLogwriter(bool echoConsole)
        {
            _echoConsole = echoConsole;
        }

        public void AppendLine(string text)
        {
            _lines.Add(text);
            if(_echoConsole) Console.WriteLine(text);
            
        }

        public List<string> ReadLines()
        {
            return _lines;
        } 
    }
}
