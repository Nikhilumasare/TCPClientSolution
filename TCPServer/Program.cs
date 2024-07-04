using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            TcpServer server = new TcpServer(8888);
            server.Start();
        }
    }
}
