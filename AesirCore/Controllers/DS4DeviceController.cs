using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AesirLights.Structs;
using CommonLib;
using DS4Library;

namespace AesirCore.Controllers
{
    public class DS4DeviceController : DeviceController
    {
        public DS4DeviceController() : base("N/A")
        {

        }
        DS4Device device;
        public DS4DeviceController(int idx, DS4Device _device) : base($"DualShock4 {idx}")
        {
            device = _device;
        }

        public override void Initialize()
        {
        }

        public override void SetRgb(Rgb rgb)
        {
            base.SetRgb(rgb);
            device.LightBarColor = rgb;
        }
    }
}
