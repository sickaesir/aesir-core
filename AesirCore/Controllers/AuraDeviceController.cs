using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AesirCore.Aura;
using AesirLights.Structs;
using CommonLib;

namespace AesirCore.Controllers
{
    public class AuraDeviceController : DeviceController
    {
        public uint ledIdx { get; private set; }
        public LedType ledType { get; private set; }
        public bool invalidated { get; private set; }

        public AuraDeviceController(string name, uint led, LedType type) : base(name)
        {
            ledIdx = led;
            ledType = type;
        }

        public override void Initialize()
        {
            SetRgb(new Rgb(0, 0, 0));
        }

        public override void SetRgb(Rgb _rgb)
        {
            base.SetRgb(_rgb);
            invalidated = true;
            //AuraSyncSharp.SetLedRgb(ledType, ledIdx, rgb);
        }

        public void OnLedSet()
        {
            invalidated = false;
        }
    }
}
