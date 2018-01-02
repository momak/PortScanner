using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace PortScanner
{
    class PortScanner:IDisposable
    {
        private static CountdownEvent _countdown;
        private static SemaphoreSlim _semaphore;
        private string _host;
        private PortList _portList;
        private ILog _log;

        bool _disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                handle?.Dispose();

                _countdown.Dispose();

            }

            _disposed = true;
        }
        ~PortScanner()
        {
            Dispose(false);
        }

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
        public void StartWork(int threadCtr)
        {
            Task[] tasks = new Task[threadCtr];
            for (int i = 0; i < threadCtr; i++)
            {
                tasks[i] = Task.Factory.StartNew(ScanPorts);

            }
            Task.WaitAll(tasks);

            //_semaphore = new SemaphoreSlim(threadCtr);
            //_countdown = new CountdownEvent(threadCtr);

            //for (int i = 0; i < threadCtr; i++)
            //{
            //    Thread th = new Thread(new ThreadStart(ScanPorts));
            //    th.Name = i.ToString();
            //    th.Start();
            //}

            //_countdown.Wait();
        }

        private void ScanPorts()
        {
            int port;
            TcpClient tcp = new TcpClient();
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
                }
                _log.WriteLine($"Ending scan for IP: {_host}");
            }
            _countdown.Signal();
        }
    }
}
