using AesirLights.Controllers;
using CommonLib;
using HidLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AesirLights
{
    class CorsairHubManager
    {
        private Dictionary<HidDevice, List<CorsairHubController>> Controllers;
        public CorsairHubManager()
        {
            CreateControllerDevice();
            new Thread(WorkerThread).Start();
        }

        public List<CorsairHubController> GetControllers()
        {
            List<CorsairHubController> controllers = new List<CorsairHubController>();

            foreach(var entry in Controllers)
            {
                controllers.AddRange(entry.Value);
            }

            return controllers;
        }

        private byte[] CreateDeviceReport(Rgb[] rgbs)
        {
            byte[] output = new byte[] { 
                0x51, 0xA8, 0x00, 0x00, 
                rgbs[0].r, rgbs[0].g, rgbs[0].b,
                rgbs[1].r, rgbs[1].g, rgbs[1].b,
                rgbs[2].r, rgbs[2].g, rgbs[2].b,
                rgbs[3].r, rgbs[3].g, rgbs[3].b, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00

            };

            return output;
        }


        private byte[] StartDeviceReport()
        {
            return new byte[]
            {
                0x41, 0x80, 0x00, 0x00,
                0x00, 0x00, 0x00,
                0x00, 0x00, 0x00,
                0x00, 0x00, 0x00,
                0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
        }

        private void CreateControllerDevice()
        {
            Controllers = new Dictionary<HidDevice, List<CorsairHubController>>();
            var devices = HidDevices.Enumerate(0x2516, 0x004f);

            byte idx = 0;

            foreach(var device in devices)
            {
                device.OpenDevice(false);

                if (!device.IsOpen) continue;

                byte[] data = StartDeviceReport(); // CreateDeviceReport(new Rgb[] { new Rgb(), new Rgb(), new Rgb(), new Rgb() });

                if(!device.WriteFile(data))
                {
                    device.CloseDevice();
                    continue;
                }

                Controllers[device] = new List<CorsairHubController>();

                for(int i = 0; i < 4; i++)
                {
                    Controllers[device].Add(new CorsairHubController(idx++));
                }
            }
        }

        private void WorkerThread()
        {
            while(true)
            {
                ApplyControllersRgb();
                Thread.Sleep(5);
            }
        }


        private void ApplyControllersRgb()
        {

            foreach(var entry in Controllers)
            {
                if (!entry.Key.IsOpen || !entry.Key.IsConnected) continue;

                Rgb[] rgbs = entry.Value.Select(k => k.CurrentRGB).ToArray();

                byte[] data = CreateDeviceReport(rgbs);

                entry.Key.WriteFile(data);
            }
            
        }

    }
}
