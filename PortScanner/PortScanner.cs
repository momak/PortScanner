using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortScanner
{
    class PortScanner
    {
        private static CountdownEvent _countdown;
        private static SemaphoreSlim _semaphore;
        private string _host;
        private PortList _portList;
        private ILog _log;

        public PortScanner(string host, int portStart, int portStop, ILog log)
        {
            this._host = host;
            this._portList = new PortList(portStart, portStop);
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
            //Task[] tasks = new Task[threadCtr];
            //for (int i = 0; i < threadCtr; i++)
            //{
            //    tasks[i] = Task.Factory.StartNew(Run);

            //}
            //Task.WaitAll(tasks);

            //_semaphore = new SemaphoreSlim(threadCtr);
            //_countdown = new CountdownEvent(threadCtr);

            for (int i = 0; i < threadCtr; i++)
            {
                Thread th = new Thread(new ThreadStart(Run));
                th.Name = i.ToString();
                th.Start();
            }

            //_countdown.Wait();
        }

        public void Run()
        {
           
            int port;
            TcpClient tcp = new TcpClient();
            //tcp.ReceiveTimeout = 1000;
            while ((port = _portList.GetNext()) != -1)
            {
                try
                {
                    _log.WriteLine($"Scanning {port}@{_host}");
                    tcp = new TcpClient(_host, port);
                    _log.WriteLine($"Port {port}@{_host} is open");
                }
                catch (Exception e)
                {
                    _log.WriteLine($"Port {port}@{_host} not open");
                    continue;
                }
                finally
                {
                    try
                    {
                        tcp.Close();
                    }
                    catch (Exception e)
                    {

                    }
                    _log.WriteLine($"Ending scan for IP: {port}@{_host}");
                }
            }

            //_semaphore.Release();
            //_countdown.Signal();
            //_log.WriteLine($"Remainig threads: {_countdown.CurrentCount}");
            //_log.WriteLine($"Ending scan for IP: {port}@{_host}");
        }
    }
}
