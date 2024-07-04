using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCPServer;

public class TcpServer
{
    private TcpListener _listener;
    private int _port;
    private string _key = ConfigurationManager.AppSettings["AesKey"].Trim();
    private string _iv = ConfigurationManager.AppSettings["AesIv"].Trim();

    public Dictionary<string, Dictionary<string, int>> _dict = new Dictionary<string, Dictionary<string, int>>()
        {
            {"SetA", new Dictionary<string, int>(){ {"One", 1 }, { "Two", 2 } } },
            {"SetB", new Dictionary<string, int>(){ {"Three", 3 }, { "Four", 4 } } },
            {"SetC", new Dictionary<string, int>(){ {"Five", 5 }, { "Six", 6 } } },
            {"SetD", new Dictionary<string, int>(){ {"Seven", 7 }, { "Eight", 8 } } },
            {"SetE", new Dictionary<string, int>(){ {"Nine", 9 }, { "Ten", 10 } } }

        };
    public TcpServer(int port)
    {
        this._port = port;
    }

    public void Start()
    {
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        _listener = new TcpListener(ip , _port);
        _listener.Start();
        Console.WriteLine($"Server started. Listening on port {_port}");

        try
        {
            while (true)
            {
                // Accept a client connection
                TcpClient client = _listener.AcceptTcpClient();
                Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");

                // Handle client communication in a separate thread
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }

    private async void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        StringBuilder builder = new StringBuilder();
        AESEncryptionDecryption aesEncryption = new AESEncryptionDecryption(_key, _iv);
        try
        {
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break; // Client disconnected
                }

                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received from client: {data}");
                string decryptedData = aesEncryption.DecryptString(data);
                Console.WriteLine($"Decrypted Data: { decryptedData}");

                int res = await CheckServerCollection(decryptedData);
                if (res == 0)
                {
                    string msg = "Invalid Input!";
                    string encryptedData = aesEncryption.EncryptString(msg);
                    byte[] response = Encoding.UTF8.GetBytes(encryptedData);
                    stream.Write(response, 0, response.Length);
                    Console.WriteLine($"Sent to client: {encryptedData}");
                    Thread.Sleep(1000);
                }
                else
                {
                    for (int i = 1; i <= res; i++)
                    {
                        string dateTime = DateTime.Now.ToString();
                        string encryptedData = aesEncryption.EncryptString(dateTime);
                        byte[] response = Encoding.UTF8.GetBytes(encryptedData);
                        stream.Write(response, 0, response.Length);
                        Console.WriteLine($"Sent to client: {encryptedData}");
                        Thread.Sleep(1000);
                    }
                }
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }

    public void Stop()
    {
        _listener.Stop();
    }

    public async Task<int> CheckServerCollection(string clientString)
    {
        int res = 0;
        try
        {
            string[] keys = clientString.Split('-');
            if (keys.Length.Equals(2))
            {
                string firstKey = keys[0];
                string secondKey = keys[1];

                if (_dict.ContainsKey(firstKey))
                {
                    var dict = _dict[firstKey];
                    if (dict.ContainsKey(secondKey))
                        res = dict[secondKey];
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return res;
    }
}
