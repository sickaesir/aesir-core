using AesirCore.Effects;
using AesirLights.Structs;
using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AesirCore.Controllers
{
    public interface IDeviceController
    {
        void Initialize();
        void SetRgb(Rgb rgb);
    }

    public class DeviceController : IDeviceController
    {
        private object locker = new object();
        public string Name { private set; get; }
        public EffectBase Effect { private set; get; } = null;
        public Rgb CurrentRGB { private set; get; } = new Rgb();

        public int Brightness { private set; get; } = 100;

        private int currentBrightness = 100;

        private EffectBase nextEffect = null;

        public DeviceController(string name)
        {
            Name = name;
            Effect = new StaticEffect(new Rgb());
            new Thread(DeviceLedWorker).Start();
        }

        public void SetEffect(EffectBase effect, bool transition)
        {
            lock(locker)
            {
                var oldEffect = this.Effect;

                if (transition)
                {
                    Rgb currentRgb = this.Effect.GetEffectRgb();
                    this.nextEffect = effect;
                    this.nextEffect.Pause();
                    this.Effect = new TransitionEffect(currentRgb, nextEffect.GetEffectRgb());
                    this.Effect.Pause();
                    ((TransitionEffect)this.Effect).SetPercentage(0);
                    ((TransitionEffect)this.Effect).SetEffectSpeed(9900);
                    ((TransitionEffect)this.Effect).TransitionEnded += TransitionEffect_TransitionEnded;

                    this.Effect.Resume();
                }
                else
                {
                    // this.Effect.Pause();
                    this.Effect = effect;
                    //  this.Effect.Resume();
                }

                if (oldEffect != null && oldEffect != this.Effect)
                    oldEffect.Dispose();

            }

        }

        private void TransitionEffect_TransitionEnded(object sender, EventArgs e)
        {
            //if(this.Effect != null)
            //    this.Effect.Dispose();

            lock(locker)
            {
                this.Effect = this.nextEffect;
                if (this.Effect != null)
                    this.Effect.Resume();
                this.nextEffect = null;
            }

        }

        public void PrintControllerLine()
        {
            Rgb rgb = CurrentRGB;
            int rV = (int)((float)rgb.r).Map(0, 255, 0, 10);
            int gV = (int)((float)rgb.g).Map(0, 255, 0, 10);
            int bV = (int)((float)rgb.b).Map(0, 255, 0, 10);

            ConsoleColor color = Console.ForegroundColor;

            Console.WriteLine($"{Name}\t");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("R: ");
            for (int i = 0; i < 10; i++)
                Console.Write(rV > i ? "=" : "-");
            Console.Write("   ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("G: ");
            for (int i = 0; i < 10; i++)
                Console.Write(gV > i ? "=" : "-");
            Console.Write("   ");


            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("B: ");
            for (int i = 0; i < 10; i++)
                Console.Write(bV > i ? "=" : "-");
            Console.Write("   \r\n\n");
        }

        private void DeviceLedWorker()
        {
            while(true)
            {
                Rgb newRgb = this.CurrentRGB;
                if(Effect != null)
                {
                    lock(locker)
                    {
                        newRgb = Effect.GetEffectRgb();
                    }
                }

                if(currentBrightness > Brightness)
                {
                    currentBrightness--;
                }
                else if(currentBrightness < Brightness)
                {
                    currentBrightness++;
                }

                float rate = currentBrightness / 100.0f;
                newRgb = new Rgb((byte)(newRgb.r * rate), (byte)(newRgb.g * rate), (byte)(newRgb.b * rate));

                if (newRgb != this.CurrentRGB)
                    SetRgb(newRgb);

                Thread.Sleep(20);
            }
        }

        public virtual void Initialize()
        {
        }

        public void SetBrightness(int brightness)
        {
            Brightness = Math.Min(100, Math.Max(0, brightness));
        }

        public virtual void SetRgb(Rgb rgb)
        {
            this.CurrentRGB = rgb;
        }
    }
}
