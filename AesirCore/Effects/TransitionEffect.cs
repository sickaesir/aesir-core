using AesirLights.Structs;
using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AesirCore.Effects
{
    [Serializable]
    class TransitionEffect : EffectBase
    {
        private Rgb sourceRgb;
        private Rgb destRgb;
        private int percentage;

        public event EventHandler TransitionEnded;
        public TransitionEffect() : base("Transition")
        {
            sourceRgb = new Rgb();
            destRgb = new Rgb();
            percentage = 0;
        }

        public TransitionEffect(Rgb source, Rgb destination) : base("Transition")
        {
            sourceRgb = source;
            destRgb = destination;
            percentage = 0;
        }

        public Rgb GetSourceRgb()
        {
            return sourceRgb;
        }

        public Rgb GetDestinationRgb()
        {
            return destRgb;
        }



        public void SetSourceRgb(Rgb rgb)
        {
            sourceRgb = rgb;
        }

        public void SetDestinationRgb(Rgb rgb)
        {
            destRgb = rgb;
        }

        public void SetPercentage(int percentage)
        {
            percentage = Math.Max(0, Math.Min(100, percentage));
        }

        protected override Rgb EffectTick()
        {
            percentage++;
            if(percentage > 100)
            {
                percentage = 100;
                Pause();

                new Thread(() =>
                {
                    TransitionEnded?.Invoke(this, new EventArgs());
                }).Start();
            }
            float normalizedPercentage = percentage / 100.0f;
            return new Rgb((byte)Helpers.Lerp(sourceRgb.r, destRgb.r, normalizedPercentage), (byte)Helpers.Lerp(sourceRgb.g, destRgb.g, normalizedPercentage), (byte)Helpers.Lerp(sourceRgb.b, destRgb.b, normalizedPercentage));
        }

        protected override void ParseEffectSettings(dynamic data)
        {
            if (data == null)
                return;

            if (data.source != null)
                SetSourceRgb(new Rgb((byte)data.source.r, (byte)data.source.g, (byte)data.source.b));

            if (data.dest != null)
                SetDestinationRgb(new Rgb((byte)data.dest.r, (byte)data.dest.g, (byte)data.dest.b));
        }

    }
}
