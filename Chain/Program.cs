using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Chain
{
    class Program
    {
        private static Socket sender;
        private static Socket listener;
        private static void CreateSocket(int port, string nextHost, int nextPort) 
        {   
            IPAddress listenerIP = IPAddress.Any;
            IPEndPoint localEndPoint = new IPEndPoint(listenerIP, port);
            listener = new Socket(
                listenerIP.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);

            IPAddress senderIP = null;
            if (nextHost == "localhost") 
            {
                senderIP = IPAddress.Loopback;
            }
            else 
            {
                senderIP = IPAddress.Parse(nextHost);
            }
            IPEndPoint remoteEP = new IPEndPoint(senderIP, nextPort);
            sender = new Socket(
                senderIP.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            ConnectSocket(remoteEP);
        }

        private static void ConnectSocket(IPEndPoint remoteEP) 
        {
            while (true)
            {
                try
                {
                    sender.Connect(remoteEP);
                    return;
                }
                catch (SocketException)
                {
                    Thread.Sleep(1000);
                }
            }
        }
        private static void Initiator(string x)
        {
            sender.Send(Encoding.UTF8.GetBytes(x));

            Socket handler = listener.Accept();
            byte[] buf = new byte[1024];
            string data = null;
            do
            {
                int bytes = handler.Receive(buf);
                data += Encoding.UTF8.GetString(buf, 0, bytes);
            }
            while (handler.Available > 0);
            string y = Encoding.UTF8.GetString(buf);
            x = y;
            Console.WriteLine(x);

            sender.Send(Encoding.UTF8.GetBytes(Math.Max(Convert.ToInt32(x), Convert.ToInt32(y)).ToString()));

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
            Console.ReadLine();
        }
        private static void Normal(string x)
        {
            Socket handler = listener.Accept(); 
            byte[] buf = new byte[4];
            string data = null;
            do
            {
                int bytes = handler.Receive(buf);
                data += Encoding.UTF8.GetString(buf, 0, bytes);
            }
            while (handler.Available > 0);
            string y = Encoding.UTF8.GetString(buf);

            sender.Send(Encoding.UTF8.GetBytes(Math.Max(Convert.ToInt32(x), Convert.ToInt32(y)).ToString()));

            data = null;
            do
            {
                int bytes = handler.Receive(buf);
                data += Encoding.UTF8.GetString(buf, 0, bytes);
            }
            while (handler.Available > 0);
            
            sender.Send(buf);
            Console.WriteLine(Encoding.UTF8.GetString(buf));

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
            Console.ReadLine();
        }
        static void Main(string[] args)
        {
            CreateSocket(Convert.ToInt32(args[0]), args[1], Convert.ToInt32(args[2]));
            bool first = args.Length == 4 && args[3] == "true";
            string x = Console.ReadLine();
            if (first) 
            {
                Initiator(x);
                
            }
            else
            {
                Normal(x);
            }

            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }
}
