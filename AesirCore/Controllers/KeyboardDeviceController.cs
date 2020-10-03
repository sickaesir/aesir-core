using AesirCore.Controllers;
using CommonLib;
using HidLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AesirLights.Controllers
{
    class KeyboardDeviceController : DeviceController
    {

        // vid: 195d pid: 2047
        HidDevice keyboard;
        public KeyboardDeviceController() : base("Keyboard")
        {
            keyboard = null;
        }

        public override void Initialize()
        {
            var devices = HidDevices.Enumerate(0x195d, 0x2047);


            foreach(var device in devices)
            {
                device.OpenDevice(false);

                if (!device.IsOpen) continue;

                if(TestDevice(device))
                {
                    keyboard = device;
                    break;
                }

                device.CloseDevice();

            }
        }

        public override void SetRgb(Rgb rgb)
        {
            base.SetRgb(rgb);
        }

        private bool TestDevice(HidDevice device)
        {
            return true;
        }
    }
}
