using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConsoleApplication1
{
    class Server
    {
        public static void Main()
        {
            Thread tcp = new Thread(Tcp);
            Thread udp = new Thread(Udp);

            tcp.Start();
            udp.Start();
        }

        static void Udp()
        {
            bool done = false;

            int udPort = 11000;
            UdpClient udpListener = new UdpClient(udPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, udPort);

            string received_data;
            byte[] receive_byte_array;

            try
            {
                while (!done)
                {
                    Console.WriteLine("Waiting for broadcast");
                    receive_byte_array = udpListener.Receive(ref groupEP);
                    Console.WriteLine("Received a broadcast from {0}", groupEP.ToString());
                    received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                    Console.WriteLine("data follows \n{0}\n\n", received_data);

                    string stringResponse = "Roger that";
                    byte[] byteResponse = Encoding.ASCII.GetBytes(stringResponse);
                    udpListener.Send(byteResponse, byteResponse.Length, groupEP);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Tcp()
        {
            //IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];

            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
            TcpClient client = default(TcpClient);

            try
            {
                server.Start();
                Console.WriteLine("Server has started on 127.0.0.1:{0}.{1}Waiting for a connection...", 8000, Environment.NewLine);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Read();
            }

            while (true)
            {
                client = server.AcceptTcpClient();

                byte[] received = new byte[100];
                NetworkStream stream = client.GetStream();

                stream.Read(received, 0, received.Length);

                StringBuilder msg = new StringBuilder();

                foreach (byte b in received)
                {
                    if (b.Equals(00))
                    {
                        break;
                    }
                    else
                    {
                        msg.Append(Convert.ToChar(b).ToString());
                    }
                }

                Console.WriteLine("{0} : ({1})", msg.ToString(), msg.Length.ToString());
            }
        }
    }
}