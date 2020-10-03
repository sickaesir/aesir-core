using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ScarlettLib
{
    public class ScarlettService
    {
        Guid clientKey;
        UdpClient client;


        public ScarlettService()
        {
            CreateUdpClient();
            new Thread(DiscoveryBroadcastWorker).Start();
            new Thread(DiscoveryReceiverWorker).Start();
        }


        public event EventHandler OnDeviceAdded;

        private void CreateUdpClient()
        {
            clientKey = Guid.NewGuid();
            while (true)
            {
                int port = new Random((int)(DateTime.Now.Ticks & 0xFFFFFFFF)).Next(1, 65535);

                try
                {
                    client = new UdpClient(port);
                }
                catch(Exception)
                {
                    client = null;
                    continue;
                }

                break;
            }
        }


        private void DiscoveryReceiverWorker()
        {
            while(true)
            {
                IPEndPoint broadcastEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 30096);
                byte[] bytes = client.Receive(ref broadcastEndpoint);

                Regex regex = new Regex(@"port='(\d+)'\/>");

                string str = Encoding.UTF8.GetString(bytes);

                if(regex.IsMatch(str))
                {
                    string match = regex.Match(str).Groups[1].Value;

                    int port = int.Parse(match);

                    ScarlettDevice device = new ScarlettDevice(port);

                    OnDeviceAdded?.Invoke(device, new EventArgs());

                    return;
                }

                Thread.Sleep(1000);
            }
        }

        private void DiscoveryBroadcastWorker()
        {
            while(true)
            {
                IPEndPoint broadcastEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 30096);
                byte[] discoveryPacket = Helpers.GetPacket($"<client-discovery app=\"SAFFIRE-CONTROL\" version=\"4\"/>");
                client.Send(discoveryPacket, discoveryPacket.Length, broadcastEndpoint);

                Thread.Sleep(1000);
            }
        }
    }
}
