using AesirLights.Structs;
using CommonLib;
using System;
using System.Threading;

namespace AesirCore.Effects
{
    [Serializable]
    public class EffectBase : IDisposable
    {
        protected Rgb rgb;
        private Thread workerThread;
        private bool disposed;
        private bool paused = false;
        private int sleepTime;
        private long lastTick;
        public string Name { get; private set; }
        public EffectBase(string name)
        {
            sleepTime = 50;
            Name = name;
            rgb = new Rgb(0, 0, 0);
            disposed = false;
            lastTick = DateTime.Now.Ticks;
            workerThread = new Thread(ThreadWorker);
            workerThread.Start();
        }
        public void Pause()
        {
            this.paused = true;
        }
        public void Resume()
        {
            this.paused = false;
        }
        public Rgb GetEffectRgb()
        {
            return rgb;
        }
        public void SetEffectSpeed(int speed)
        {
            sleepTime = Math.Max(1, Math.Min(10000, 10000 - speed));
        }
        private void ThreadWorker()
        {
            while (!disposed)
            {
                if (lastTick < DateTime.Now.Ticks + sleepTime && !paused)
                {
                    lastTick = DateTime.Now.Ticks;
                    if (!paused)
                    {
                        try
                        {
                            rgb = EffectTick();
                        }
                        catch(Exception)
                        {

                        }
                    }
                }
                Thread.Sleep(10);
            }
        }
        protected virtual Rgb EffectTick()
        {
            return new Rgb(0, 0, 0);
        }
        public virtual void Dispose()
        {
            disposed = true;
            workerThread.Join();
        }
        protected virtual void ParseEffectSettings(dynamic data)
        {
        }
        public void ParseSettingsFromDynamic(dynamic data)
        {
            if (data.speed != null)
                SetEffectSpeed((int)data.speed);
            ParseEffectSettings(data);
        }
    }
}
