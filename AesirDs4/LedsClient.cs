using CommonLib;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace AesirDs4
{
    class LedsClient
    {
        private Dictionary<Guid, string> controllers;
        private Dictionary<Guid, string> effects;
        private ConcurrentDictionary<int, dynamic> responses;
        private Random random;
        WebSocket client;
        public bool Connected { get; private set; }
        public LedsClient()
        {
            Connected = false;
            random = new Random();
            effects = new Dictionary<Guid, string>();
            controllers = new Dictionary<Guid, string>();
            responses = new ConcurrentDictionary<int, dynamic>();
            client = new WebSocket("ws://localhost:61427");
            client.OnOpen += OnClientOpen;
            client.OnMessage += OnClientMessage;
            client.Log.Output = (a, b) => { };
            new Thread(ConnectWorker).Start();
        }


        private void ConnectWorker()
        {
            while(true)
            {
                if(Connected && client.IsAlive)
                {
                    Thread.Sleep(500);
                    continue;
                }

                Connected = false;
                responses.Clear();
                try
                {
                    client.Connect();
                }
                catch(Exception)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                if(!client.IsAlive)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                Console.WriteLine($"Socket connected to light service");
                Connected = true;
            }
        }

        private void OnClientMessage(object sender, MessageEventArgs e)
        {
            dynamic jsonData = null;
            try
            {
                jsonData = JsonConvert.DeserializeObject(e.Data);
            }
            catch(Exception)
            {

            }

            int requestId = jsonData.reqId;
            dynamic data = jsonData.data;

            responses.TryAdd(requestId, data);

        }

        private void OnClientOpen(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                foreach (var effect in SendClientCommand(0))
                {
                    effects.Add(Guid.Parse((string)effect.Key), (string)effect.Value);
                    Console.WriteLine($"Registered effect {effect.Value}");
                }
            }).Start();

            new Thread(() =>
            {
                foreach (var controller in SendClientCommand(1))
                {
                    controllers.Add(Guid.Parse((string)controller.Key), (string)controller.Value);
                    Console.WriteLine($"Registered controller {controller.Value}");
                }
            }).Start();
        }

        private void SendClientCommandWithNoResponse(byte cmd, dynamic data = null)
        {
            client.SendAsync(JsonConvert.SerializeObject(new { cmd = cmd, data = data, req = 0 }), (b) => { });
        }

        private dynamic SendClientCommand(byte cmd, dynamic data = null)
        {
            int reqId = random.Next();

            client.Send(JsonConvert.SerializeObject(new { cmd = cmd, data = data, req = reqId }));

            dynamic response = null;
            while(true)
            {
                if(!responses.ContainsKey(reqId))
                {
                    Thread.Sleep(500);
                    continue;
                }

                response = responses[reqId];
                responses.TryRemove(reqId, out dynamic _);
                break;
            }

            if (response == null)
                return null;

            return response;
        }

        public void SetAllLedsRgb(Rgb rgb)
        {
            SendClientCommandWithNoResponse(2, new
            {
                effectGuid = effects.FirstOrDefault(k => k.Value.ToLower() == "static").Key,
                effectSettings = new
                {
                    skipTransition = true,
                    rgb = rgb
                }
            });
        }
    }
}
