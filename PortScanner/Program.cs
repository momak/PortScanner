using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PortScanner
{
    class Program
    {
        public const int StartPort = IPEndPoint.MinPort;
        public const int EndPort = IPEndPoint.MaxPort;
        public const string Localhost = "127.0.0.1";

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

            _writter = new Writter(WriteMode.Both, _outputFile);

            _startAddress = GetIPaddress(_commandLineOptions.StartIpAddress);
            _writter.WriteLine($"Start IP address {_startAddress}");

            _endAddress = GetIPaddress(_commandLineOptions.EndIpAddress);
            _writter.WriteLine($"End IP address {_endAddress}");

            _maxThread = _commandLineOptions.MaxThreadCount;
            _writter.WriteLine($"Maximum threads allocated {_maxThread}");

            _writter.WriteLine($"Starting port {StartPort}");

            _writter.WriteLine($"Ending port {EndPort}");

            IPhelper ipHelper = new IPhelper();

            var range = ipHelper.GetIPRange(_startAddress, _endAddress);

            //foreach (string ip in range)
            Parallel.ForEach(range, ip =>
            {
                _writter.WriteLine($"Starting scan for IP: {ip}");

                PortScanner pScanner = new PortScanner(ip, StartPort, EndPort, _writter);
                pScanner.Start(_maxThread);

            });

        }

        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return IPAddress.Parse(ip.ToString());
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private static IPAddress GetIPaddress(string ipAddress)
        {
            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                try
                {
                    return IPAddress.Parse(ipAddress);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{ipAddress} is not a valid ip address: " + e.Message);
                }
            }
            return GetLocalIPAddress();
        }

        private static void CreateLocation(string pathToFolder)
        {
            if (!string.IsNullOrWhiteSpace(pathToFolder))
            {
                if (!Directory.Exists(pathToFolder))
                {
                    Directory.CreateDirectory(pathToFolder);
                }
                _outputFile = pathToFolder + "\\" + DateTime.Now.ToString("yyyy_MM-dd-HH-mm") + ".txt";
            }
            else
            {
                throw new Exception("Cannot create output directory/file!");
            }
        }

    }
}
