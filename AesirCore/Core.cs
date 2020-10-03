using AesirLights.Structs;
using CommonLib;
using Fleck;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AesirCore.Effects;
using System.Threading.Tasks;
using AesirLights.Effects;

namespace AesirCore
{
    public class Core
    {
        public LedsManager Leds;
        public WebSocketManager WebSocket;
        public Core()
        {
            AudioAnalyzer.Init();
            AuraSyncSharp.Init();

            Leds = new LedsManager();
            WebSocket = new WebSocketManager();
            InitializeControllers();
            Console.Clear();
        }

        void InitializeControllers()
        {
            new Thread(PrintControllerThread).Start();


            TransitionEffect transition = new TransitionEffect(new Rgb(40, 20, 100), new Rgb(0, 40, 100));
            EventHandler tranEnd = null;
            
            tranEnd = (object sender, EventArgs ea) =>
            {
                Rgb start = transition.GetSourceRgb();
                Rgb end = transition.GetDestinationRgb();

                transition.Pause();
                transition.Dispose();

                transition = new TransitionEffect(end, start);
                transition.TransitionEnded += tranEnd;
            };
            transition.TransitionEnded += tranEnd;

            Leds.SetControllerEffectOnAllControllers(new RainbowEffect(), false);
            return;


            var controllers = Leds.GetControllers();

            // screens
            {
                Leds.SetControllerEffect(controllers[2].Key, new StaticEffect(new Rgb(255, 255, 255)), false);
            }


            // wardrobe
            {
                AudioEffect audioEffect = new AudioEffect();
                audioEffect.SetBassColor(new Rgb(0, 255, 255));
                audioEffect.SetBackgroundEffect(new StaticEffect(new Rgb(0, 255, 0)));
                audioEffect.SetIdleEffect(new StaticEffect(new Rgb(0, 0, 0)));

                Leds.SetControllerEffect(controllers[1].Key, audioEffect, false);
            }

            // speakers
            {
                AudioEffect audioEffect = new AudioEffect();
                audioEffect.SetBassColor(new Rgb(0, 0, 255));
                audioEffect.SetBackgroundEffect(new StaticEffect(new Rgb(0, 255, 255)));
                audioEffect.SetIdleEffect(new StaticEffect(new Rgb(0, 0, 0)));

                Leds.SetControllerEffect(controllers[4].Key, audioEffect, false);
            }


            // room
            {
                Leds.SetControllerEffect(controllers[6].Key, new StaticEffect(new Rgb(0, 0, 255)), false);
            }

            // mirror
            {
                AudioEffect audioEffect = new AudioEffect();
                audioEffect.SetBassColor(new Rgb(255, 0, 0));
                audioEffect.SetBackgroundEffect(new StaticEffect(new Rgb(0, 255, 255)));
                audioEffect.SetIdleEffect(new StaticEffect(new Rgb(0, 0, 0)));

                Leds.SetControllerEffect(controllers[8].Key, audioEffect, false);
            }

            // room
            {
                ColorCycleEffect cycle = new ColorCycleEffect();
                cycle.SetEffectSpeed(5000);

                Leds.SetControllerEffect(controllers[12].Key, cycle, false);
                Leds.SetControllerBrightness(controllers[12].Key, 15);

            }

            // under 
            {
                Leds.SetControllerEffect(controllers[3].Key, new StaticEffect(new Rgb(8, 45, 15)), false);
                Leds.SetControllerBrightness(controllers[3].Key, 50);

            }


            return;

            // mobo strip
            Leds.SetControllerEffect(controllers[8].Key, new StaticEffect(new Rgb(0, 40, 100)), false);

            // mirror
            Leds.SetControllerEffect(controllers[4].Key, new StaticEffect(new Rgb(0, 0, 40)), false);

            // chandelier
            Leds.SetControllerEffect(controllers[3].Key, new RainbowEffect(), false);

            // audio card
            //Leds.SetControllerEffect(controllers[11].Key, new RainbowEffect(), false);

            return;

            // mouse
            Leds.SetControllerEffect(controllers[0].Key, new RainbowEffect());

                                                                                                     

            // room strip
            Leds.SetControllerEffect(controllers[2].Key, new StaticEffect(new Rgb(200, 0, 200)));






            // mobo top
            Leds.SetControllerEffect(controllers[5].Key, new BreathingEffect(new Rgb(255, 0, 255)));



            // mobo rog
            Leds.SetControllerEffect(controllers[6].Key, new ColorCycleEffect());



            // mobo pci
            Leds.SetControllerEffect(controllers[7].Key, new StaticEffect(new Rgb(0, 255, 0)));






            // vga
            Leds.SetControllerEffect(controllers[9].Key, new StaticEffect(new Rgb(255, 0, 255)));

        }

        void PrintControllerThread()
        {
            Console.Clear();
            while (true)
            {
                Console.SetCursorPosition(0, 0);
                Leds.PrintControllers();


                Thread.Sleep(50);
            }
        }
    }
}
