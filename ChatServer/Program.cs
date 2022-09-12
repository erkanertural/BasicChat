using ChatLibrary;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("=========================\r\n    SIMPLE CHAT SERVER \r\n=========================\r\n \r\n \r\n");
            Server server = new Server();
            Task.Run(() => server.Listen()).Wait();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " > Server Started...");
            Console.ResetColor();
           Console.WriteLine( "  Info : Brodcast for all clients cmd : *  ");
            Console.ResetColor();
            Task.Run(() => server.StartChat());
            while (true)
            {
                string cmd = Console.ReadLine();
                if (cmd == "*")
                {
                    Task.Run(()=> server.Broadcast());
                }
                else if (cmd.ToLower() == "exit")
                    break;
            }

        }
    }
}
