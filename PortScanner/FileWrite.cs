using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace PortScanner
{
    class FileWrite : ILog
    {
        private string _outputFile;
        private object locker = new object();

        public FileWrite(string outputFile)
        {
            _outputFile = outputFile;
        }


        public void WriteLine(string message)
        {
            try
            {
                lock (locker)
                {
                    using (StreamWriter writer = new StreamWriter(_outputFile, true))
                    {
                        writer.WriteLine(message);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
