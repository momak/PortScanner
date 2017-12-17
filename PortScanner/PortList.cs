using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace PortScanner
{
    class PortList
    {
        readonly Object thisLock = new Object();
        private int start;
        private int stop;
        private int ptr;

        public PortList(int start, int stop)
        {
            this.start = start;
            this.stop = stop;
            this.ptr = start;
        }
        public PortList() : this(IPEndPoint.MinPort, IPEndPoint.MaxPort)
        {
        }

        public bool HasMore()
        {
            return (stop - ptr) >= 0;
        }
        public int GetNext()
        {
            //Monitor.Enter(thisLock);
            //try
            //{
            //    if (HasMore())
            //        return ptr++;
            //    return -1;
            //}
            //finally
            //{
            //    Monitor.Exit(thisLock);
            //}

            lock (thisLock)
            {
                if (HasMore())
                    return ptr++;
                return -1;
            }
        }
    }
}
