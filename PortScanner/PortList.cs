using System;
using System.Net;

namespace PortScanner
{
    class PortList
    {
        readonly Object thisLock = new Object();

        private int _start;
        private int _stop;
        private int _ptr;

        public PortList(int start, int stop)
        {
            this._start = start;
            this._stop = stop;
            this._ptr = start;
        }
        public PortList() : this(IPEndPoint.MinPort, IPEndPoint.MaxPort)
        {
        }

        public bool HasMore()
        {
            return (_stop - _ptr) >= 0;
        }
        public int GetNext()
        {
            lock (thisLock)
            {
                if (HasMore())
                    return _ptr++;
                return -1;
            }
        }
    }
}
