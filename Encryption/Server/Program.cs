using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        private static int port = 3999;
        private static IPAddress ip = IPAddress.Parse("127.0.0.1");
        private static List<Socket> sockets = new List<Socket>();

        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEndPoint = new IPEndPoint(ip, port);
            serverSocket.Bind(serverEndPoint);
            serverSocket.Listen(10);

            Console.WriteLine("SERVER STARTED");

            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                sockets.Add(clientSocket);

                ThreadPool.QueueUserWorkItem(
                    (object obj) =>
                    {
                        var socket = obj as Socket;

                        if (socket.Connected)
                            Console.WriteLine($"Client {socket.GetHashCode()} is connected");
                        else
                            return;

                        byte[] buffer = new byte[1024000];

                        while (true)
                        {
                            try
                            {
                                int length = socket.Receive(buffer);
                                foreach (var client in sockets)
                                {
                                    client.Send(buffer, 0, length, SocketFlags.None);
                                }
                            }
                            catch
                            {
                                Console.WriteLine($"Client {socket.GetHashCode()} is disconnected");
                                break;
                            }
                        }

                        socket.Close();

                    }, clientSocket);
            }
        }
    }
}
