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
    class AudioEffect : EffectBase
    {
        float Max { get; set; } = float.MinValue;
        float Min { get; set; } = float.MaxValue;
        float ColorMax { get; set; } = float.MinValue;
        float ColorMin { get; set; } = float.MaxValue;
        private EffectBase backgroundEffect;
        private EffectBase idleEffect;
        private Rgb bassColor;
        public AudioEffect() : base("Music")
        {
            idleEffect = null;
            bassColor = new Rgb();
            backgroundEffect = null;
        }

        public void SetBackgroundEffect(EffectBase effect, bool disposeOldEffect = true)
        {
            if(backgroundEffect != null && disposeOldEffect)
            {
                backgroundEffect.Dispose();
            }

            backgroundEffect = effect;
        }

        public void SetIdleEffect(EffectBase effect, bool disposeOldEffect = true)
        {
            if(idleEffect != null && disposeOldEffect)
            {
                idleEffect.Dispose();
            }

            idleEffect = effect;
        }

        public void SetBassColor(Rgb color)
        {
            //ColorMax = float.MinValue;
            //ColorMin = float.MaxValue;
            bassColor = color;
        }

        protected override void ParseEffectSettings(dynamic data)
        {
            if (data.bassColor != null && data.bassColor.rgb != null && data.bassColor.rgb.r != null && data.bassColor.rgb.g != null && data.bassColor.rgb.b != null)
                SetBassColor(new Rgb((byte)data.bassColor.rgb.r, (byte)data.bassColor.rgb.g, (byte)data.bassColor.rgb.b));

          //  if(data.backgroundEffect != null && data.backgroundEffect.guid != null)

        }


        Rgb CalculateColor(float[] data, Rgb baseColor)
        {
            int pX = 60;

            // Sub Arrays
            var aX = data.SubArray(0, pX);

            float X = aX.Sum();

            ColorMin = Math.Min(ColorMin, X);
            if (ColorMax < X)
                ColorMax = X;// + 0.001f;
            else
                ColorMax -= 0.0001f;

            float bassVal = (float)X.Map(ColorMin, ColorMax, 0, 100);

            

            return new Rgb((byte)Helpers.Lerp(baseColor.r, bassColor.r, bassVal / 100.0f), (byte)Helpers.Lerp(baseColor.g, bassColor.g, bassVal / 100.0f), (byte)Helpers.Lerp(baseColor.b, bassColor.b, bassVal / 100.0f));
        }

        float CalculateIntensity(float[] FFT)
        {

            int pX = (int)(FFT.Length * 0.33);
            int pY = (int)(FFT.Length * 0.66);

            // Sub Arrays
            var aX = FFT.SubArray(0, pX);
            var aY = FFT.SubArray(pX, pY - pX);
            var aZ = FFT.SubArray(pY, FFT.Length - pY);

            // Sub Arrays Average
            float X = 0, Y = 0, Z = 0;
            float[] values = new float[3];

            if (aX.Length > 0)
            {
                X = aX.Sum();
                values[0] = X;
            }

            if (aY.Length > 0)
            {
                Y = aY.Sum();
                values[1] = Y;
            }

            if (aZ.Length > 0)
            {
                Z = aZ.Sum();
                values[2] = Z;
            }

            // Get min value
            var min = values.Min();
            if (min < Min)
                Min = min;

            // Get max value
            var max = values.Max();
            if (max > Max)
                Max = max;
            //else
            //    Max -= 0.00001f;


            float mX = (float)(((aX.Length > 0) ? (int)Math.Ceiling(X.Map(Min, Max, 0, 255)) : 0) / 255.0);
            float mY = (float)(((aY.Length > 0) ? (int)Math.Ceiling(Y.Map(Min, Max, 0, 255)) : 0) / 255.0);
            float mZ = (float)(((aZ.Length > 0) ? (int)Math.Ceiling(Z.Map(Min, Max, 0, 255)) : 0) / 255.0);

            return Math.Max(mX, Math.Max(mY, mZ));
        }

        protected override Rgb EffectTick()
        {

            Rgb backgroundRgb = backgroundEffect == null ? new Rgb() : backgroundEffect.GetEffectRgb();


            float[] soundData = AudioAnalyzer.SoundData;
            Rgb bg = CalculateColor(soundData, backgroundRgb);

            byte r = (byte)(CalculateIntensity(soundData) * bg.r);
            byte g = (byte)(CalculateIntensity(soundData) * bg.g);
            byte b = (byte)(CalculateIntensity(soundData) * bg.b);

            if(r == 0 && g == 0 && b == 0 && idleEffect != null)
            {
                return idleEffect.GetEffectRgb();
            }


            return new Rgb(r, g, b);
        }

        public override void Dispose()
        {
            if (backgroundEffect != null)
                backgroundEffect.Dispose();

            base.Dispose();
        }
    }
}
