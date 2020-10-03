using CommonLib;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AesirCoreApi
{
    public class WebSocket
    {
        ClientWebSocket client;
        ConcurrentQueue<string> outCommands;
        ConcurrentDictionary<int, dynamic> inCommands;
        Random rand;

        Dictionary<string, Guid> controllers;
        Dictionary<string, Guid> effects;
        public WebSocket()
        {
            client = new ClientWebSocket();
            outCommands = new ConcurrentQueue<string>();
            inCommands = new ConcurrentDictionary<int, dynamic>();
            rand = new Random((int)(DateTime.Now.Ticks & 0xFFFFFFFF));
        }

        public async Task Start()
        {
            while(true)
            {
                try
                {
                    await client.ConnectAsync(new Uri("ws://localhost:61427"), CancellationToken.None);
                }
                catch(Exception)
                {
                    continue;
                }

                break;
            }

            Console.WriteLine("Connected to Core Websocket");
            Task.Run(ReceiveWorker).ConfigureAwait(false).GetAwaiter();
            Task.Run(SendWorker).ConfigureAwait(false).GetAwaiter();
            await Bootstrap();
        }

        private async Task Bootstrap()
        {
            dynamic serviceControllers = await SendMessage("GetControllers");
            controllers = new Dictionary<string, Guid>();
            foreach(var entry in serviceControllers)
            {
                string name = ((string)entry.Value).ToLower();
                Guid key = (Guid)entry.Key;
                Console.WriteLine($"{name} ({key}) controller added");
                controllers.Add(name, key);
            }

            dynamic serviceEffects = await SendMessage("GetEffects");
            effects = new Dictionary<string, Guid>();
            foreach (var entry in serviceEffects)
            {
                string name = ((string)entry.Value).ToLower();
                Guid key = (Guid)entry.Key;
                Console.WriteLine($"{name} ({key}) effect added");

                effects.Add(name, key);
            }
        }

        private async Task<dynamic> SendMessage(string cmd, dynamic data = null)
        {
            int reqId = rand.Next();
            string outData = JsonConvert.SerializeObject(new { req = reqId, cmd, data });

            outCommands.Enqueue(outData);
            inCommands.TryAdd(reqId, null);

            for(int i = 0; i < 10; i++)
            {
                await Task.Delay(1000);

                if (!inCommands.ContainsKey(reqId))
                    return null;

                if (inCommands[reqId] == null)
                    continue;

                inCommands.TryRemove(reqId, out dynamic rtn);

                return rtn;

            }

            return null;
        }

        private async Task SendWorker()
        {
            while(true)
            {
                if(!outCommands.TryDequeue(out string res))
                {
                    await Task.Delay(100);
                    continue;
                }

                ArraySegment<byte> bytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(res));

                await client.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);

            }
        }

        private async Task ReceiveWorker()
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[10240]);

            while(true)
            {
                using(var ms = new MemoryStream())
                {
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await client.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);

                    if (result.MessageType != WebSocketMessageType.Text)
                        break;

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                        OnMessageReceived(await reader.ReadToEndAsync());

                }
                await Task.Delay(100);
            }
        }

        public Task ToggleAllControllers(bool state)
        {
            return ToggleController("all", state);
        }

        public async Task ToggleController(string controller, bool state)
        {
            Guid? guid = null;

            if (controller != "all")
                guid = controllers[controller.ToLower()];

            SetControllerBrightness(controller, state ? 40 : 0);

            await SendMessage("SetControllerEffect", new
            {
                controllerGuid = guid,
                effectGuid = effects["static"],
                effectSettings = new
                {
                    rgb = new
                    {
                        r = state ? 200 : 0,
                        g = state ? 200 : 0,
                        b = state ? 200 : 0
                    },

                    speed = 1
                }
            });
        }

        public Task SetControllerBrightness(string controller, int brightness)
        {
            Guid? guid = null;

            if (controller != "all")
                guid = controllers[controller.ToLower()];

            return SendMessage("SetControllerBrightness", new
            {
                controllerGuid = guid,
                brightness
            });
        }

        public Task SetControllerColor(string controller, byte r, byte g, byte b)
        {
            Guid? guid = null;

            if (controller != "all")
                guid = controllers[controller.ToLower()];

            return SendMessage("SetControllerEffect", new
            {
                controllerGuid = guid,
                effectGuid = effects["static"],
                effectSettings = new
                {
                    rgb = new
                    {
                        r,
                        g,
                        b
                    },

                    speed = 1
                }
            });
        }

        private void OnMessageReceived(string data)
        {
            dynamic obj = JsonConvert.DeserializeObject(data);

            int reqId = (int)obj.reqId;

            if (!inCommands.ContainsKey(reqId)) return;

            inCommands[reqId] = obj.data;

        }
    }
}
