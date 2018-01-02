using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortScanner
{
    class Program
    {
        public const string Localhost = "127.0.0.1";

        public static int StartPort = IPEndPoint.MinPort;
        public static int EndPort = IPEndPoint.MaxPort;

        private static CommandLineOptions _commandLineOptions;
        private static IPAddress _startAddress;
        private static IPAddress _endAddress;
        private static int _maxThread;
        private static string _outputFile;

        private static Writter _writter;


        static void Main(string[] args)
        {
            var commandLineParser = CommandLineOptions.Setup();
            var result = commandLineParser.Parse(args);
            if (CommandLineOptions.HandleStandardResult(result))
                return;
            _commandLineOptions = commandLineParser.Object;

            CreateLocation(_commandLineOptions.OutputLocation);
            IPhelper ipHelper = new IPhelper();
            _writter = new Writter(WriteMode.Both, _outputFile);

            _writter.WriteLine($"Start application {DateTime.Now:yyyy.MM.dd HH:mm:sss}");
            _writter.WriteLine(Environment.NewLine);

            _writter.WriteLine($"Output location: {_outputFile}");

            _startAddress = ipHelper.GetIPaddress(_commandLineOptions.StartIpAddress);
            _writter.WriteLine($"Start IP address {_startAddress}");

            StartPort = SetPort(_commandLineOptions.StartPort);
            EndPort = SetPort(_commandLineOptions.EndPort);

            _endAddress = ipHelper.GetIPaddress(_commandLineOptions.EndIpAddress);
            _writter.WriteLine($"End IP address {_endAddress}");

            _maxThread = _commandLineOptions.MaxThreadCount;
            _writter.WriteLine($"Maximum threads allocated {_maxThread}");

            _writter.WriteLine($"Starting port {StartPort}");
            _writter.WriteLine($"Ending port {EndPort}");

            _writter.WriteLine(Environment.NewLine);
            var range = ipHelper.GetIPRange(_startAddress, _endAddress);

         
            //RunScan(range, ipHelper);
            RunScanParallel(range, ipHelper);
            
            _writter.WriteLine(Environment.NewLine);
            _writter.WriteLine($"Exiting application {DateTime.Now:yyyy.MM.dd HH:mm:sss}");
        }

        private static void RunScan(IEnumerable<string> range, IPhelper ipHelper)
        {
            foreach (string ip in range)
            {
                if (ipHelper.Ping(ip))
                {
                    _writter.WriteLine($"Starting scan for IP: {ip} @ {DateTime.Now:yyyy.MM.dd HH:mm:sss}");

                    PortScanner pScanner = new PortScanner(ip, StartPort, EndPort, _writter);
                    pScanner.StartWork(_maxThread);
                }
                else
                {
                    _writter.WriteLine($"Host {ip} is not active @ {DateTime.Now:yyyy.MM.dd HH:mm:sss}");
                }
            }
        }

        private static void RunScanParallel(IEnumerable<string> range, IPhelper ipHelper)
        {
            Parallel.ForEach(range, ip =>
            {
                if (ipHelper.Ping(ip))
                {
                    _writter.WriteLine($"Starting scan for IP: {ip} @ {DateTime.Now:yyyy.MM.dd HH:mm:sss}");
                    
                    PortScanner pScanner = new PortScanner(ip, StartPort, EndPort, _writter);
                    pScanner.StartWork(_maxThread);
                }
                else
                {
                    _writter.WriteLine($"Host {ip} is not active @ {DateTime.Now:yyyy.MM.dd HH:mm:sss}");
                }
            });
        }

        private static void CreateLocation(string pathToFolder)
        {
            if (!string.IsNullOrWhiteSpace(pathToFolder))
            {
                if (!Directory.Exists(pathToFolder))
                    Directory.CreateDirectory(pathToFolder);

                _outputFile = pathToFolder + "\\" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm") + ".txt";

                if (File.Exists(_outputFile))
                    File.Delete(_outputFile);
            }
            else
            {
                throw new Exception("Cannot create output directory/file!");
            }
        }

        private static int SetPort(string port)
        {
            if (!string.IsNullOrWhiteSpace(port))
                return int.Parse(port);

            throw new Exception("Not a valid Port number");
        }

    }
}
