using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TCP_server_concurrent
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("::::::::::::::::::::::::::::::SERVER::::::::::::::::::::::");
            //TcpListener listener = new TcpListener(System.Net.IPAddress.Loopback, 7);
            //TcpListener listener = new TcpListener(IPAddress.Parse("192.168.104.150"), 7);
            TcpListener listener = new TcpListener(IPAddress.Any, 7);
            listener.Start();
            
            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();

                Task.Run(() => { HandleClient(socket); });
            }


        }

        public static void HandleClient(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);

            while (true)
            {
                string message = reader.ReadLine();

                if (message == "Exit")
                { break; }

                Console.WriteLine("Client Wrote : " + message);
                writer.WriteLine(message.ToUpper());
                writer.Flush();
            }

            socket.Close();
        }
    }
}
