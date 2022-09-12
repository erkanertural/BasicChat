using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary
{
    public class SocketUser
    {

        public string UserName { get; set; }
        public int SpamCounter { get; set; }
        public DateTime LastMessageSendTime { get; set; }
    }
}
