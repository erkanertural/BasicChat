using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class Extension
    {
        public static byte[] GetBytes(this string data)
        {
            byte[] buffer = new byte[1024];
            return Encoding.UTF8.GetBytes(data);
        }
        public static string GetString(this byte[] data, int length)
        {
            byte[] buffer = new byte[1024];
            return Encoding.UTF8.GetString(data, 0, length);
        }
        public static string Read(this Socket s)
        {
            byte[] buffer = new byte[1024];
            var l = s.Receive(buffer, SocketFlags.None);
            string resp = buffer.GetString(l);
            return resp;

        }
    }
}
