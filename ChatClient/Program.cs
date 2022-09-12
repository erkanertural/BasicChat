
using ChatLibrary;
using System;
using System.Threading.Tasks;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Write("=========================\r\n    SIMPLE CHAT CLIENT \r\n=========================\r\n \r\n \r\nLogin Name :");
            string loginName = Console.ReadLine();
            Client client = new Client();
            client.Connect(loginName);
            Task.Run(() => client.StartChat());

            while (true)
            {
                string cmd = Console.ReadLine();
                if (cmd.ToLower() == "exit")
                    break;
                client.SendMessage($"{SocketMessageProtocol.Data} { cmd}");
            }

        }


    }
}
