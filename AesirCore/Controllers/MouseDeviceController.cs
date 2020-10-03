using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AesirLights.Structs;
using CommonLib;
using HidLibrary;

namespace AesirCore.Controllers
{
    public class MouseDeviceController : DeviceController
    {
        HidDevice mouse;
        public MouseDeviceController() : base("Mouse")
        {

        }

        public override void Initialize()
        {
            var devices = HidDevices.Enumerate(0x4d9, 0xfa59);

            foreach(var device in devices)
            {
                device.OpenDevice(false);

                if (device.IsOpen)
                {
                    if (!TestDevice(device))
                    {
                        device.CloseDevice();
                        continue;
                    }


                    mouse = device;
                    break;
                }
            }
            SetRgb(new Rgb(0, 0, 0));
        }

        bool TestDevice(HidDevice device)
        {
            return device.WriteFeatureData(new byte[] { 0x02, 0x04, (byte)(255), (byte)(255), (byte)(255), 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        }

        public override void SetRgb(Rgb _rgb)
        {
            base.SetRgb(_rgb);
            if (mouse == null) return;
            var data = new byte[] { 0x02, 0x04, (byte)(255 - CurrentRGB.r), (byte)(255 - CurrentRGB.g), (byte)(255 - CurrentRGB.b), 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            mouse.WriteFeatureData(data);

        }
    }
}
