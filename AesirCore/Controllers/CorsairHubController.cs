using AesirCore.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AesirLights.Controllers
{
    class CorsairHubController : DeviceController
    {
        public readonly byte Index;
        public CorsairHubController(byte index) : base($"RGB_Strip_{index + 1}")
        {
            Index = index;
        }


    }
}
