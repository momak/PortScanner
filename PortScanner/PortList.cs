using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortScanner
{
    class PortList
    {
        private int start;
        private int stop;
        private int ptr;

        public PortList(int start, int stop)
        {
            this.start = start;
            this.stop = stop;
            this.ptr = start;
        }
        public PortList() : this(1, 65535)
        {
        }

        public bool hasMore()
        {
            return (stop - ptr) >= 0;
        }
        public int getNext()
        {
            if (hasMore())
                return ptr++;
            return -1;
        }
    }
}
