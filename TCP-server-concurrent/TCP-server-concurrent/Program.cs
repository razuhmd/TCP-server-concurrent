using ClassLibrary;
using Newtonsoft.Json;
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
            Console.WriteLine("::::::::::::::::::::::::::::::SERVER IS AVAILABLE FOR CLIENT REQUEST::::::::::::::::::::::");
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
                Console.WriteLine("Client is connected....");
                string message = reader.ReadLine();

                if (message == "Exit")
                {
                    Console.WriteLine(">>>>>>>>>Client is disconnected now....");
                    break;
                }
                else
                {
                    Console.WriteLine("Client Wrote : " + message);
                    // writer.WriteLine(message);
                    switch (message)
                    {
                        case "GetAll":

                            var bookskList = manager.GetAll();
                            reader.ReadLine();
                            writer.WriteLine("::::::::::All The Books::::::::");
                            string serializedObject = JsonConvert.SerializeObject(bookskList);
                            writer.WriteLine(serializedObject);
                            //foreach (var book in bookskList)
                            //{
                            //    writer.WriteLine();
                            //    writer.WriteLine($"Title : {book.Title}");
                            //    writer.WriteLine($"Author : {book.Author}");
                            //    writer.WriteLine($"Number of pages : {book.PageNumber}");
                            //    writer.WriteLine($"ISBN : {book.ISBN13}");
                            //    writer.WriteLine("---------------------------");
                            //}
                            break;

                        case "Get":

                            //writer.WriteLine("Insert ISBN: ");                           
                            string readISBN = reader.ReadLine();
                            //writer.WriteLine(readISBN);
                            Book bookByISBN = manager.GetById(readISBN);
                            if (bookByISBN != null)
                            {
                                writer.WriteLine();
                                writer.WriteLine("Book details for inserted ISBN::::: ");
                                writer.WriteLine($"Title : {bookByISBN.Title}");
                                writer.WriteLine($"Author : {bookByISBN.Author}");
                                writer.WriteLine($"Number of pages : {bookByISBN.PageNumber}");
                                writer.WriteLine($"ISBN : {bookByISBN.ISBN13}");
                                writer.WriteLine("---------------------------");
                                
                            }
                            else
                            {
                                throw new ArgumentException("Invalid ISBN!!!!");
                            }
                            break;

                        case "Save": // {"Title": "UML", "Author": "Larman", "PageNumber": 654} 

                            var readJson = reader.ReadLine();
                            if (readJson != null)
                            {
                                var jsonBook = JsonConvert.DeserializeObject<Book>(readJson);
                                Book newBook = manager.Create(jsonBook.Title, jsonBook.Author, jsonBook.PageNumber);
                                writer.WriteLine();
                                writer.WriteLine("::::Newly Added book details::::");
                                writer.WriteLine();
                                writer.WriteLine($"Title : {newBook.Title}");
                                writer.WriteLine($"Author : {newBook.Author}");
                                writer.WriteLine($"Number of pages : {newBook.PageNumber}");
                                writer.WriteLine($"ISBN : {newBook.ISBN13}");
                                writer.WriteLine("---------------------------");
                            }
                            else
                            {
                                throw new ArgumentException("Invalid JSON...");
                            }
                            break;
                    }
                }
                
                writer.Flush();
            }

            socket.Close();
        }
    }
}
