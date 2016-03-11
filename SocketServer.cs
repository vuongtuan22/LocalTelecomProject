using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace crmphone
{
    class SocketServer
    {
        private Int32 port;
        //public delegate void CtiMsgHander(string Message);
        // public event CtiMsgHander SocketEvent;
        public event EventHandler<socketEventArgs> SocketEvent;
        public SocketServer(Int32 port)
        {
            Console.WriteLine("Socket Server Start:" + port);
            this.port = port;
        }
        public void socketServerStart()
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            TcpListener serverSocket = new TcpListener(localAddr, this.port);
            serverSocket.Start();
            Console.WriteLine(" >> Server Started");
            while ((true))
            {
                try
                {
                    TcpClient clientSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine(" >> Accept connection from client");
                    NetworkStream networkStream = clientSocket.GetStream();
                    //byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
                    byte[] bytesFrom = new byte[32];
                    // networkStream.Read(bytesFrom, 0, clientSocket.ReceiveBufferSize);
                    networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    List<byte> list = new List<byte>();
                    for (int i = 0; i < bytesFrom.Length; i++)
                    {
                        if (bytesFrom[i] < 32) break;
                        list.Add(bytesFrom[i]);
                    }
                    byte[] arr = list.ToArray();
                    //int bytesRead = networkStream.Read(bytesFrom, 0, clientSocket.ReceiveBufferSize);
                    //string dataReceived = Encoding.ASCII.GetString(bytesFrom, 0, bytesRead);
                    string dataFromClient = System.Text.Encoding.ASCII.GetString(arr);
                    //dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    //string dataFromClient = new string(bytesFrom);
                    Console.WriteLine(" >> Data from client - " + dataFromClient);
                    if (SocketEvent != null)
                    {
                        SocketEvent(this, new socketEventArgs(dataFromClient));
                    }
                    //string serverResponse = "Last Message from client" + dataFromClient;
                    //Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    //networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    //Console.WriteLine(" >> " + serverResponse);
                    clientSocket.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

        }




    }
}
