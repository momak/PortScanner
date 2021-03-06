﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace PortScanner
{
    public class IPhelper
    {
        public IEnumerable<string> GetIPRange(IPAddress startIP, IPAddress endIP)
        {
            uint sIP = ipToUint(startIP.GetAddressBytes());
            uint eIP = ipToUint(endIP.GetAddressBytes());
            while (sIP <= eIP)
            {
                yield return new IPAddress(reverseBytesArray(sIP)).ToString();
                sIP++;
            }
        }

        /* Convert bytes array to 32 bit long value */
        protected uint ipToUint(byte[] ipBytes)
        {
            ByteConverter bConvert = new ByteConverter();
            uint ipUint = 0;

            int shift = 24; // indicates number of bits left for shifting
            foreach (byte b in ipBytes)
            {
                if (ipUint == 0)
                {
                    ipUint = (uint)bConvert.ConvertTo(b, typeof(uint)) << shift;
                    shift -= 8;
                    continue;
                }

                if (shift >= 8)
                    ipUint += (uint)bConvert.ConvertTo(b, typeof(uint)) << shift;
                else
                    ipUint += (uint)bConvert.ConvertTo(b, typeof(uint));

                shift -= 8;
            }

            return ipUint;
        }

        /* reverse byte order in array */
        protected uint reverseBytesArray(uint ip)
        {
            byte[] bytes = BitConverter.GetBytes(ip);
            bytes = bytes.Reverse().ToArray();
            return (uint)BitConverter.ToInt32(bytes, 0);
        }

        public IPAddress GetLocalIPAddress()
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

        public IPAddress GetIPaddress(string ipAddress)
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

        public bool Ping(IPAddress address)
        {
            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(address);
                return reply.Status == IPStatus.Success;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool Ping(string address)
        {
            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(address);
                return reply.Status == IPStatus.Success;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
