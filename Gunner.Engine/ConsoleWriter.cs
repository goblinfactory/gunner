using System;

namespace Gunner.Engine
{
    public class ConsoleWriter : ILogWriter
    {
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public void Write(string text)
        {
            Console.Write(text);
        }
    }
}