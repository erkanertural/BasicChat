using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLibrary
{
    public class Client
    {
        public event Action<string> DataRead;
        Socket socket = null;
        string sendingData = "";
        public Client()
        {
            this.DataRead += Client_DataRead;
        }

        private void Client_DataRead(string obj)
        {
            if (obj.Contains(SocketMessageProtocol.Broadcast))
                Console.ForegroundColor = ConsoleColor.Red;
            else if (obj.Contains(SocketMessageProtocol.Spam))
                Console.ForegroundColor = ConsoleColor.DarkRed;
            else if (obj.Contains(SocketMessageProtocol.Block))
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Server:" + obj);
            Console.ResetColor();
        }

        public Socket ClientSocket { get => socket; }

        public void Connect(string loginName)
        {
            socket = SocketExt.Connect();
            SendMessage(SocketMessageProtocol.Login + loginName);
        }

        public void SendMessage(string data)
        {
            sendingData = data;
        }

        public async void StartChat()
        {
            while (true)
            {
                try
                {
                    if (ClientSocket.Connected)
                    {
                        if (sendingData.Length > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} > Sending..: \"{sendingData}\"");
                            Console.ResetColor();
                            byte[] messageBytes = Encoding.UTF8.GetBytes(sendingData);
                            await ClientSocket.SendAsync(messageBytes, SocketFlags.None);
                            sendingData = "";
                        }
                        if (ClientSocket.Available > 0)
                        {
                            byte[] buffer = new byte[1024];
                            int received = await ClientSocket.ReceiveAsync(buffer, SocketFlags.None);
                            string data = Encoding.UTF8.GetString(buffer, 0, received);
                            DataRead(data);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                Thread.Sleep(50);
            }
        }
    }
}
