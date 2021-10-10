using SimpleRestService.Managers;
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
            Console.WriteLine();
            //TcpListener listener = new TcpListener(System.Net.IPAddress.Loopback, 7);
            //TcpListener listener = new TcpListener(IPAddress.Parse("192.168.104.150"), 7);
            TcpListener listener = new TcpListener(IPAddress.Any, 4646);
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
            BooksManager manager = new BooksManager();

            while (true)
            {
                string message = reader.ReadLine();

                if (message == "Exit") { break; }

                Console.WriteLine("Client Wrote : " + message);
               // writer.WriteLine(message);
                switch (message)
                    {
                    case "GetAll":
                        var bookskList = manager.GetAll();
                        foreach(var book in bookskList)
                        {
                            writer.WriteLine();
                            writer.WriteLine(book.Title);
                            writer.WriteLine(book.Author);
                            writer.WriteLine(book.PageNumber);
                            writer.WriteLine(book.ISBN13);
                        }
                        
                        break;

                    case "Get":
                        writer.WriteLine("Insert ISBN: ");
                        break;
                    }
                
                writer.Flush();
            }

            socket.Close();
        }
    }
}
