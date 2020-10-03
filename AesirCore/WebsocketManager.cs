using AesirCore.Attributes;
using AesirCore.Effects;
using AesirLights.Structs;
using Fleck;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AesirCore
{
    public class WebSocketManager : WebSocketServer
    {
        private ConcurrentDictionary<Guid, IWebSocketConnection> Connections;
        public WebSocketManager() : base("ws://0.0.0.0:61427", true)
        {
            Connections = new ConcurrentDictionary<Guid, IWebSocketConnection>();
            ListenerSocket.NoDelay = true;
            RestartAfterListenError = true;
            Start(connection =>
            {
                connection.OnBinary = (data) => OnBinary(connection, data);
                connection.OnClose = () => OnClose(connection);
                connection.OnMessage = (data) => OnMessage(connection, data);
                connection.OnOpen = () => OnOpen(connection);
                connection.OnError = (exception) => OnError(connection, exception);

                Console.Clear();
            });
        }


        private void OnError(IWebSocketConnection connection, Exception ex)
        {
            try
            {
                connection.Close();
            }
            catch(Exception)
            {

            }
        }

        private void OnOpen(IWebSocketConnection connection)
        {
            Connections.TryAdd(connection.ConnectionInfo.Id, connection);
        }

        private void OnMessage(IWebSocketConnection connection, string data)
        {
            dynamic jsonData = null; 
            
            try
            {
                jsonData = JsonConvert.DeserializeObject<dynamic>(data);
            }
            catch(Exception ex)
            {
                OnError(connection, ex);
                return;
            }

            if(jsonData.req == null)
            {
                OnError(connection, new NullReferenceException());
                return;
            }

            int requestId = (int)jsonData.req;

            if(jsonData.cmd == null)
            {
                OnError(connection, new NullReferenceException());
                return;
            }

            CommandType cmd = (CommandType)jsonData.cmd;

            dynamic inData = (dynamic)jsonData.data;

            dynamic response = InvokeHandler(cmd, inData);

            if(response == null)
            {
                OnError(connection, new InvalidOperationException());
                return;
            }

            SendMessageToConnection(connection.ConnectionInfo.Id, new { reqId = requestId, data = response });

        }

        private dynamic InvokeHandler(CommandType type, dynamic data)
        {
            MethodInfo[] methods = typeof(WebSocketManager).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

            IEnumerable<MethodInfo> handlerMethods = methods.Where(entry => {
               return entry.CustomAttributes.ToList().Exists(c => c.AttributeType == typeof(WSHandlerAttribute));
            });

            MethodInfo callerMethod = handlerMethods.FirstOrDefault(k => k.Name == $"On{type}");

            if(callerMethod == null)
                return null;

            dynamic result = callerMethod.Invoke(this, new object[] { data });

            return result;

        }

        [WSHandler]
        private dynamic OnSetControllerBrightness(dynamic data)
        {
            if (data == null)
                return new { error = "invalid data" };

            if (data.brightness == null)
                return new { error = "specify brightness" };

            int brightness = (int)data.brightness;

            if (data.controllerGuid != null)
            {
                if (!Guid.TryParse((string)data.controllerGuid, out Guid controllerGuid))
                {
                    return new { error = "invalid controllerGuid" };
                }


                AesirCore.LightsCore.Leds.SetControllerBrightness(controllerGuid, brightness);
            }
            else
                AesirCore.LightsCore.Leds.SetControllerBrightnessOnAllControllers(brightness);


            return new { status = "ok" };
        }

        [WSHandler]
        private dynamic OnSetControllerEffect(dynamic data)
        {
            if (data == null || data.effectGuid == null)
                return new { error = "specify both effectGuid" };

            if(!Guid.TryParse((string)data.effectGuid, out Guid effectGuid))
            {
                return new { error = "invalid effectGuid" };
            }


            Guid controllerGuid = Guid.Empty;

            if(data.controllerGuid != null)
            {
                if (!Guid.TryParse((string)data.controllerGuid, out controllerGuid))
                {
                    return new { error = "invalid controllerGuid" };
                }

                List<KeyValuePair<Guid, string>> controllers = AesirCore.LightsCore.Leds.GetControllers();

                if (!controllers.Exists(k => k.Key == controllerGuid))
                {
                    return new { error = "controller not found" };
                }
            }

            EffectBase effectInstance = AesirCore.LightsCore.Leds.CreateEffectFromGuid<EffectBase>(effectGuid);

            if (effectInstance == null)
                return new { error = "effect not found" };

            bool transition = true;

            if (data.effectSettings != null)
            {
                transition = !(data.effectSettings.skipTransition != null && data.effectSettings.skipTransition == true);
                effectInstance.ParseSettingsFromDynamic(data.effectSettings);
            }

            

            if (controllerGuid == Guid.Empty)
                AesirCore.LightsCore.Leds.SetControllerEffectOnAllControllers(effectInstance, transition);
            else
                AesirCore.LightsCore.Leds.SetControllerEffect(controllerGuid, effectInstance, transition);

            return new { status = "ok" };
        }

        [WSHandler]
        private dynamic OnGetEffects(dynamic data)
        {
            return AesirCore.LightsCore.Leds.GetEffects();
        }

        [WSHandler]
        private dynamic OnGetControllers(dynamic data)
        {
            return AesirCore.LightsCore.Leds.GetControllers();
        }

        private void OnClose(IWebSocketConnection connection)
        {
            Connections.TryRemove(connection.ConnectionInfo.Id, out var _);
        }

        private void OnBinary(IWebSocketConnection connection, byte[] data)
        {
            SendMessageToConnection(connection.ConnectionInfo.Id, new { error = "binary data is not supported" });
        }

        private bool SendMessageToConnection(Guid guid, dynamic data)
        {
            if (!Connections.ContainsKey(guid)) return false;

            IWebSocketConnection connection = Connections[guid];

            try
            {
                connection.Send(JsonConvert.SerializeObject(data));
            }
            catch(Exception ex)
            {
                OnError(connection, ex);
                return false;
            }

            return true;


        }

        private void SendMessageToAllConnections(dynamic data)
        {
            foreach(var connection in Connections)
            {
                SendMessageToConnection(connection.Key, data);
            }
        }
    }
}
