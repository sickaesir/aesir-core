using AesirLights.Structs;
using CommonLib;
using CSCore;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.Streams;
using CSCore.Streams.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AesirCore
{
    static class AudioAnalyzer
    {
        static IWaveSource _source;
        static PitchShifter _pitchShifter;
        static ISampleSource source;
        static FftProvider provider;
        static WasapiLoopbackCapture _soundIn;
        static SingleBlockNotificationStream notificationSource;
        static SoundInSource soundInSource;
        public static float[] SoundData { get; private set; }

        public static void Init()
        {
            _soundIn = new WasapiLoopbackCapture();
            _soundIn.Initialize();

            soundInSource = new SoundInSource(_soundIn);
            source = soundInSource.ToSampleSource().AppendSource(x => new PitchShifter(x), out _pitchShifter);


            provider = new FftProvider(source.WaveFormat.Channels, FftSize.Fft16384);

            notificationSource = new SingleBlockNotificationStream(source);
            //pass the intercepted samples as input data to the spectrumprovider (which will calculate a fft based on them)
            notificationSource.SingleBlockRead += (s, a) =>
            {
                provider.Add(a.Left, a.Right);
            };


            _source = notificationSource.ToWaveSource(32);



            // We need to read from our source otherwise SingleBlockRead is never called and our spectrum provider is not populated
            byte[] buffer = new byte[_source.WaveFormat.BytesPerSecond / 2];
            soundInSource.DataAvailable += (s, aEvent) =>
            {
                int read;
                while ((read = _source.Read(buffer, 0, buffer.Length)) > 0) ;

            };
            SoundData = new float[(int)provider.FftSize];
            _soundIn.Start();
            new Thread(Worker).Start();
        }

        private static void Worker()
        {
            while(true)
            {
                float[] data = new float[(int)provider.FftSize];
                provider.GetFftData(data);
                SoundData = data.NormalizeArray();
                Thread.Sleep(5);
            }
        }
    }
}
