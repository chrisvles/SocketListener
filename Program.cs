using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SocketListener
{
    class Program
    {
        //Allow for multiple Clients
        private static List<Socket> clients = new List<Socket>();
        private static Thread listening_thread;
        private static TcpListener listener;
      

        //Set IP and Port depending on where job is being recieved from
        private static String ListeningIP = "127.0.0.1";
        private static int port = 8001;

        //Program starts, listen on thread
        static void Main(string[] args)
        {
            listening_thread = new Thread(new ThreadStart(ListeningThread));
            listening_thread.Start();
        }



        private static void ListeningThread() 
        {
    
            //Set up endpoint
            IPAddress EndPointIP = IPAddress.Parse(ListeningIP);
            //Initiate listener
            listener = new TcpListener(EndPointIP, port);
            //Try to start, Exception writes line to console
            try
            {
                listener.Start();
            }
            catch (Exception e) { Console.WriteLine("couldn't bind to port " + port + " -> " + e.Message); return; }
            //Loop over port, listening for response
            //Sleep thread every 30 ms so it doesn't freak out
            while (true)
            {
                //Handles Re
                if (listener.Pending())
                    clients.Add(listener.AcceptSocket()); // won't block because pending was true

                foreach (Socket sock in clients)
                    if (sock.Poll(0, SelectMode.SelectError))
                        clients.Remove(sock);
                    else if (sock.Poll(0, SelectMode.SelectRead))
                        ParserFunction(sock, EndPointIP);

                Thread.Sleep(30);
            }
        }

        /*
         * This runs when a response is found
         * This will eventually be changed to handle data differently
         * depending on how we decide to feed it in.
         * From here the c# would could spawn up a webcrawler
         * parse information from page and probably send data to a seperate 
         * endpoint
        */
        private static void ParserFunction(Socket sock, IPAddress EndPointSel)
        {
            using (var n = new NetworkStream(sock))
            {
                while (true)
                {
                    var data_reader = new StreamReader(n);
                    var line = data_reader.ReadLine();
                    if (line.Length == 0)
                    {
                        break;
                    }
                    int index = line.IndexOf(':');
                    Console.WriteLine(line);
                }
            }
        }

    }
}
