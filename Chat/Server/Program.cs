using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Server
{
    class Program
    {
        private static List<string> history = new List<string>();
        public static void StartListening(int port)
        {
            IPAddress ipAddress = IPAddress.Any; 
            
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            Socket listener = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);

                listener.Listen(10);

                while (true)
                {
                    Socket handler = listener.Accept();

                    byte[] buf = new byte[1024];
                    string data = null;
                    do
                    {
                        int bytes = handler.Receive(buf);
                        data += Encoding.UTF8.GetString(buf, 0, bytes);
                    }
                    while (handler.Available > 0);

                    Console.WriteLine("Полученный текст: {0}", data);
                    history.Add(data);
                    
                    var jsonMessages = JsonSerializer.Serialize(history);

                    byte[] msg = Encoding.UTF8.GetBytes(jsonMessages);

                    handler.Send(msg);

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        static void Main(string[] args)
        {
            StartListening(Int32.Parse(args[0]));
        }
    }
}
