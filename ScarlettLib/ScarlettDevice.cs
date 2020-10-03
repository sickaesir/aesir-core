using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScarlettLib
{
    public class ScarlettDevice
    {
        private int servicePort;
        private TcpClient client;
        private NetworkStream stream;
        private Guid clientGuid;
        public ScarlettDevice(int port)
        {
            clientGuid = Guid.NewGuid();
            servicePort = port;
            InitTcpClient();
        }

        private void InitTcpClient()
        {
            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", servicePort);
            stream = client.GetStream();
            Handshake();
            new Thread(KeepAliveThread).Start();
        }

        private void KeepAliveThread()
        {
            while(true)
            {
                //SendPacket("<keep-alive/>");
                Thread.Sleep(1000);
            }
        }

        private void SendPacket(string packet)
        {
            byte[] packetBytes = Helpers.GetPacket(packet);

            stream.Write(packetBytes, 0, packetBytes.Length);
        }

        private void Handshake()
        {
            string handshake = $"<client-details hostname=\"Aesir-Desktop\" client-key=\"ceb7c8e3-438c-47d6-9c3b-2d6da44933f3\"/>";

            SendPacket(handshake);

            //string[] colors = new string[]
            //{
            //    "red",
            //    "amber",
            //    "green",
            //    "light blue",
            //    "blue",
            //    "pink",
            //    "light pink"
            //};

            //int i = 0;
            //while(true)
            //{
            //    ChangeLedColor(colors[i]);

            //    Console.WriteLine($"Color - {colors[i]}");

            //    i = (i + 1) % colors.Length;

            //    Thread.Sleep(1000);
            //}
        }

        public void ChangeLedColor(string colorName)
        {
            string packet = $"<set devid=\"1\"><item id=\"44\" value=\"{colorName}\"/></set>";

            SendPacket(packet);
        }
    }
}
