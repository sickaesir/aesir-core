using AesirLights.Structs;
using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AesirCore.Effects
{
    [Serializable]
    class RainbowEffect : TransitionEffect
    {
        private int cycle = 0;
        private int index = 0;
        private readonly Rgb[] rainbowColors = new Rgb[]
        {
            new Rgb(255, 0, 0),
            new Rgb(255, 127, 0),
            new Rgb(255, 255, 0),
            new Rgb(0, 255, 0),
            new Rgb(0, 0, 255),
            new Rgb(46, 43, 95),
            new Rgb(139, 0, 255)
        };

        private int GetNextIndex()
        {
            return (index + 1) % rainbowColors.Length;
        }


        public RainbowEffect()
        {
            this.rgb = EffectTick();
        }

        protected override Rgb EffectTick()
        {
            cycle++;

            if(cycle > 100)
            {
                cycle = 0;
                index = GetNextIndex();
            }

            float normalizedCycle = cycle / 100.0f;

            return new Rgb((byte)Helpers.Lerp(rainbowColors[index].r, rainbowColors[GetNextIndex()].r, normalizedCycle), 
                (byte)Helpers.Lerp(rainbowColors[index].g, rainbowColors[GetNextIndex()].g, normalizedCycle), 
                (byte)Helpers.Lerp(rainbowColors[index].b, rainbowColors[GetNextIndex()].b, normalizedCycle));

        }
    }
}
