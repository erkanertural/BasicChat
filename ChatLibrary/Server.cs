using ChatLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLibrary
{
    public class Server:IDisposable
    {

        Dictionary<Socket, SocketUser> clients = new Dictionary<Socket, SocketUser>();
        public Server()
        {

        }
        public EndPoint IpEndPoint { get; private set; }
        public List<Socket> Clients { get => clients.Keys.ToList(); }
        public Socket Listener { get; private set; }
        public async void Listen()
        {
            Listener = SocketExt.CreateListener();
            Thread.Sleep(50);
            while (true)
            {
                try
                {
                    Socket handler = await Listener.AcceptAsync();
                    clients.Add(handler, new SocketUser());
                }
                catch (Exception ex)
                {

                }
                Thread.Sleep(50);
            }
        }
        public async Task Broadcast()
        {
            Socket[] c = Clients.ToArray();
            foreach (Socket item in c)
            {
                var m = "Broadcasting : Datetime.Now -> " + DateTime.Now.ToString();
                var echoBytes = Encoding.UTF8.GetBytes(m);
                await item.SendAsync(echoBytes, SocketFlags.None);
            }
        }
        public async void StartChat()
        {
            while (true)
            {
                Thread.Sleep(50);
                try
                {
                    Socket[] c = Clients.ToArray();
                    foreach (var item in c)
                    {
                        if (item.Connected == false)
                            continue;
                        if (item.Available > 0)
                        {
                            byte[] buffer = new byte[1_024];
                            int received = await item.ReceiveAsync(buffer, SocketFlags.None);
                            string command = Encoding.UTF8.GetString(buffer, 0, received);
                            if (command.IndexOf(SocketMessageProtocol.Login) > -1)
                            {
                                string message = $" {SocketMessageProtocol.StatusOK},{SocketMessageProtocol.Login}";
                                byte[] encodes = Encoding.UTF8.GetBytes(message);
                                await item.SendAsync(encodes, 0);
                                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - Logged User :" + command);
                                if (string.IsNullOrEmpty(clients[item].UserName))
                                    clients[item].UserName = command.Replace(SocketMessageProtocol.Login, "");
                            }
                            else
                            {
                                SocketUser current = clients[item];
                                string message = $"{SocketMessageProtocol.StatusOK},{SocketMessageProtocol.Data}";
                                string userName = current.UserName;
                                if (current.LastMessageSendTime == DateTime.MinValue)
                                    current.LastMessageSendTime = DateTime.Now.AddSeconds(-2);
                                double diffTime = Math.Round((DateTime.Now - current.LastMessageSendTime).TotalMilliseconds);
                                Debug.WriteLine(diffTime);
                                if (diffTime <= 1000)
                                {
                                    if (current.SpamCounter == 0)
                                    {
                                        message = $" {SocketMessageProtocol.Spam} Too Many Message in 1 seconds ({diffTime}). If you do once again you will be blocking ";
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} > {clients[item].UserName} : <[DETECT SPAM]> ");
                                        Console.ResetColor();
                                        current.SpamCounter++;
                                        var echoBytes = Encoding.UTF8.GetBytes(message);
                                        await item.SendAsync(echoBytes, 0);
                                    }
                                    else if (current.SpamCounter > 0)
                                    {
                                        message = $" {SocketMessageProtocol.Block} You produced spam message and you have been blocking ";
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} > {clients[item].UserName} : <[BLOCK USER]> ");
                                        Console.ResetColor();
                                        var echoBytes = Encoding.UTF8.GetBytes(message);
                                        await item.SendAsync(echoBytes, 0);
                                        clients.Remove(item);
                                        item.Shutdown(SocketShutdown.Both);
                                    }
                                }
                                else
                                {
                                    current.LastMessageSendTime = DateTime.Now;
                                    byte[] encodes = Encoding.UTF8.GetBytes(message);
                                    await item.SendAsync(encodes, 0);
                                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} > {clients[item].UserName} : " + command);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public void Dispose()
        {
            Listener.Dispose();
            Clients.ForEach(o => o.Dispose());
        }
    }
}
