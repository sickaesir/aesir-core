using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AesirCoreApi
{
    public static class Api
    {
        public static WebSocket CoreSocket;

        public static async Task Init()
        {
            CoreSocket = new WebSocket();
            await CoreSocket.Start();

            await Api.CoreSocket.ToggleController("all", true);
        }
    }
}
