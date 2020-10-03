using AesirCore.Effects;
using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AesirCore.Effects
{
    [Serializable]
    class SelectEffect : EffectBase
    {
        private Rgb blink1;
        private Rgb blink2;
        private bool odd;

        public SelectEffect() : base("Select")
        {
            blink1 = new Rgb(0xff, 0, 0);
            blink2 = new Rgb(0, 0xff, 0);
            odd = false;
            SetEffectSpeed(9000);
        }

        public void SetBlinks(Rgb _blink1, Rgb _blink2)
        {
            blink1 = _blink1;
            blink2 = _blink2;
        }

        protected override Rgb EffectTick()
        {
            odd = !odd;
            return odd ? blink1 : blink2;
        }
    }
}
