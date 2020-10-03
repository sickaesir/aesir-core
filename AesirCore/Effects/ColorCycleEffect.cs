using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AesirLights.Structs;
using CommonLib;

namespace AesirCore.Effects
{
    [Serializable]
    class ColorCycleEffect : EffectBase
    {
        private int cycle;
        public ColorCycleEffect() : base("Color Cycle")
        {
            cycle = 0;
        }

        protected override Rgb EffectTick()
        {
            cycle = (int)((cycle + 1) % 360);
            return Rgb.FromHsv(cycle, 1, 1);
        }
    }
}
