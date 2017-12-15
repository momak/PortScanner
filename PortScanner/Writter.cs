using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PortScanner
{
    class Writter : ILog
    {
        private WriteMode _wm;
        private string _outputFile;
        private object locker = new object();

        public Writter(WriteMode wm, string outputFile)
        {
            _wm = wm;
            _outputFile = outputFile;
        }
        public void WriteLine(string message)
        {
            try
            {
                lock (locker)
                {
                    switch (_wm)
                    {
                        case WriteMode.Console:
                            {
                                WriteToConsole(message);
                                break;
                            }
                        case WriteMode.File:
                            {
                                WriteToFile(message);
                                break;
                            }
                        case WriteMode.Both:
                            {
                                WriteToConsole(message);
                                WriteToFile(message);
                                break;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void WriteToFile(string message)
        {
            using (StreamWriter writer = new StreamWriter(_outputFile, true))
            {
                writer.WriteLine(message);
            }
        }

        private static void WriteToConsole(string message)
        {
            Console.WriteLine(message);
        }
    }
}
