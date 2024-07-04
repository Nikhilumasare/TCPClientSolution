using System;
using System.Configuration;
using System.Net.Sockets;
using System.Text;
using TCPClient;

public class TcpClient
{
    private System.Net.Sockets.TcpClient client;
    private string _key = ConfigurationManager.AppSettings["AesKey"].Trim();
    private string _iv = ConfigurationManager.AppSettings["AesIv"].Trim();

    public TcpClient()
    {
        client = new System.Net.Sockets.TcpClient();
    }

    public void Connect(string ipAddress, int port)
    {
        try
        {
            client.Connect(ipAddress, port);
            Console.WriteLine($"Connected to server {ipAddress}:{port}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to server: {ex.Message}");
        }
    }

    public async void StartCommunication(string message)
    {
        try
        {
            AESEncryptionDecryption aesEncryption = new AESEncryptionDecryption(_key, _iv); 
            NetworkStream stream = client.GetStream();
            stream.ReadTimeout = 5 * 1000;

            // Send data to server
            string encryptedData = aesEncryption.EncryptString(message);
            byte[] data = Encoding.UTF8.GetBytes(encryptedData);
            stream.Write(data, 0, data.Length);
            Console.WriteLine($"Sent to server: {encryptedData}");

            while(true)
            {
                // Receive response from server
                byte[] buffer = new byte[1024];
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) { break; }
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string decryptedData = aesEncryption.DecryptString(response);
                    Console.WriteLine($"Received from server: {decryptedData}");
                }
                catch (Exception ex) { Console.WriteLine("Read Operation End"); break; }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error communicating with server: {ex.Message}");
            client.Close();
        }
    }

    public void CloseSocket()
    {
        client.Close();
    }
}
