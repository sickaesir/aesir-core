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
    class StaticEffect : EffectBase
    {
        public StaticEffect() : base("Static")
        {
            SetStaticRgb(new Rgb(0, 0, 0));
        }

        public StaticEffect(Rgb _rgb) : base("Static")
        {
            SetStaticRgb(_rgb);
        }

        public void SetStaticRgb(Rgb rgb)
        {
            this.rgb = rgb;
        }

        protected override Rgb EffectTick()
        {
            return rgb;
        }

        protected override void ParseEffectSettings(dynamic data)
        {
            if (data == null) return;


            if (data.rgb != null)
                SetStaticRgb(new Rgb((byte)data.rgb.r, (byte)data.rgb.g, (byte)data.rgb.b));

        }
    }
}
