using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortScanner
{
    class ConsoleWrite : ILog
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
