using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client
{
    class Program
    {
        public static void StartClient(string host, int port, string message)
        {
            try
            {
                IPAddress ipAddress = null;
                if (host == "localhost") 
                {
                    ipAddress = IPAddress.Loopback;
                }
                else 
                {
                    ipAddress = IPAddress.Parse(host);
                }

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                Socket sender = new Socket(
                    ipAddress.AddressFamily,
                    SocketType.Stream, 
                    ProtocolType.Tcp);

                try
                {
                    sender.Connect(remoteEP);
                    byte[] msg = Encoding.UTF8.GetBytes(message);
                    int bytesSent = sender.Send(msg);
                    byte[] buf = new byte[1024];
                    StringBuilder historyBuilder = new StringBuilder();
                    do
                    {
                        historyBuilder.Append(Encoding.UTF8.GetString(buf, 0, sender.Receive(buf, buf.Length, 0)));
                    }
                    while (sender.Available > 0);

                    var history = JsonSerializer.Deserialize<List<string>>(historyBuilder.ToString());
                    foreach (var m in history)
                    {
                        Console.WriteLine(m);
                    }    
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            StartClient(args[0], Int32.Parse(args[1]), args[2]);
        }
    }
}
