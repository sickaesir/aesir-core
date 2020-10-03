using AesirCore.Controllers;
using CommonLib;
using HidLibrary;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using ScarlettLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AesirLights.Controllers
{
    class ScarlettDeviceController : DeviceController
    {
        ScarlettService service;
        ScarlettDevice device;
        string lastColor = "amber";
        public ScarlettDeviceController() : base("Scarlett_Focusrite")
        {
            device = null;
            service = new ScarlettService();
            service.OnDeviceAdded += (e, a) =>
            {
                device = (ScarlettDevice)e;
            };
        }

        public override void Initialize()
        {
        }

        public override void SetRgb(Rgb rgb)
        {
            if(device != null)
            {
                switch(rgb.r)
                {
                    case 0: lastColor = "red"; break;
                    case 1: lastColor = "amber"; break;
                    case 2: lastColor = "green"; break;
                    case 3: lastColor = "light blue"; break;
                    case 4: lastColor = "blue"; break;
                    case 5: lastColor = "pink"; break;
                    case 6: lastColor = "light pink"; break;
                }

                device.ChangeLedColor(lastColor);
            }

            base.SetRgb(rgb);
        }
    }
}
