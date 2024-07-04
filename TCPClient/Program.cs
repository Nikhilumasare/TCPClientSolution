using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 8888); // Replace with server's IP address and port

            // Client starts communication with server
            //client.StartCommunication();
            Console.WriteLine("====== Menu ======\n1.Send Message \n2.Exit");
            string input = Console.ReadLine();
            while (input != "2")
            {
                Console.WriteLine("\nEnter the Input: ");
                string msg = Console.ReadLine();
                client.StartCommunication(msg);
            }
            client.CloseSocket();
        }
    }
}
