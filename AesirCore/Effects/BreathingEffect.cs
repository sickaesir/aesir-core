using AesirCore.Effects;
using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AesirLights.Effects
{
    [Serializable]
    class BreathingEffect : EffectBase
    {
        private int cycle;
        private bool direction;
        private Rgb rgb;
        public BreathingEffect(Rgb _rgb) : base("Color Cycle")
        {
            rgb = _rgb;
            cycle = 0;
            direction = true;
        }

        protected override Rgb EffectTick()
        {
            if (direction)
                cycle++;
            else
                cycle--;

            if(cycle <= 0 && !direction)
            {
                cycle = 0;
                direction = true;
            }
            else if(cycle >= 100 && direction)
            {
                cycle = 100;
                direction = false;
            }

            float normalizedBreath = cycle / 100.0f;

            return new Rgb((byte)(rgb.r * normalizedBreath), (byte)(rgb.g * normalizedBreath), (byte)(rgb.b * normalizedBreath));
        }
    }
}
