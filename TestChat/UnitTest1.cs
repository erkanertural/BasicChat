using ChatLibrary;
using NUnit.Framework;
using System.Net.Sockets;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace TestChat
{
    public class Tests
    {
        Socket socket;
        [SetUp]
        public void Setup()
        {
            Server server = new Server();
            server.Listen();
            Task.Run(() => server.StartChat());
            socket = SocketExt.Connect();
        }
        [Test]
        public void Login()
        {
            socket.Send(SocketMessageProtocol.Login.GetBytes());
            string resp = socket.Read();
            Assert.AreEqual(true, resp.Contains(SocketMessageProtocol.StatusOK));
        }
        [Test]
        public void SpamTest()
        {
            Login();
            Socket socket = SocketExt.Connect();
            socket.Send(SocketMessageProtocol.Data.GetBytes());
            socket.Read();
            socket.Send(SocketMessageProtocol.Data.GetBytes());
            string respWarning = socket.Read();
            Assert.AreEqual(true, respWarning.Contains(SocketMessageProtocol.Spam));
            socket.Send(SocketMessageProtocol.Data.GetBytes());
            string respBlock = socket.Read();
            Assert.AreEqual(true, respBlock.Contains(SocketMessageProtocol.Block));
        }
    }


}