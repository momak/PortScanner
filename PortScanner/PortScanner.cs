using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PortScanner
{
    class PortScanner
    {
        private string host;
        private PortList portList;
        private ILog _log;

        public PortScanner(string host, int portStart, int portStop, ILog log)
        {
            this.host = host;
            this.portList = new PortList(portStart, portStop);
            this._log = log;
        }

        public PortScanner(string host, ILog log)
            : this(host, Program.StartPort, Program.EndPort, log)
        {
        }

        public PortScanner(ILog log)
            : this(Program.Localhost, log)
        {
        }

        public void Start(int threadCtr)
        {
            for (int i = 0; i < threadCtr; i++)
            {
                Thread th = new Thread(new ThreadStart(Run));
                th.Start();
            }
        }
        public void Run()
        {
            int port;
            TcpClient tcp = new TcpClient();
            //tcp.ReceiveTimeout = 1000;
            while ((port = portList.getNext()) != -1)
            {
                try
                {
                    tcp = new TcpClient(host, port);
                    _log.WriteLine($"{port}@{host} is open");
                }
                catch
                {
                    continue;
                }
                finally
                {
                    try
                    {
                        tcp.Close();
                    }
                    catch
                    {
                        
                    }
                }
              
            }
        }
    }
}
