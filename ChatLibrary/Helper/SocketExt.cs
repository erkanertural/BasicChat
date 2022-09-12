using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary
{
    public static class SocketExt
    {


        static string _ip = "127.0.0.1";
        static int _port = 11000;

        public static Socket Connect()
        {
            IPAddress ip = IPAddress.Parse(_ip);
            IPEndPoint endPoint = new IPEndPoint(ip, _port);
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.ConnectAsync(endPoint).Wait();
            return socket;
        }
        public static Socket CreateListener()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(ip, 11000);
            Socket listener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(endPoint);
            listener.Listen(100);
            return listener;
        }
    }

    public static class SocketMessageProtocol
    {
        public static string Login = "<[Login]>";
        public static string Spam = "<[Spam]>";
        public static string Block = "<[Block]>";
        public static string Broadcast = "<[Broadcast]>";
        public static string StatusOK = "<[StatusOK]>";
        public static string Data = "<[Data]>";
    }
}
