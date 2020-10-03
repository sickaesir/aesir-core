using AesirCore.Effects;
using CommonLib;
using DS4Library.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AesirCore.Menu
{
    public class LedsMenu : MenuBase
    {
        private enum State : int
        {
            SelectController,
            SelectEffect,
            SelectEffectSettings
        }

        private Guid selectedController;
        private EffectBase selectedEffectBackup;
        private EffectBase currentSelectedEffect;
        private List<Guid> controllers;
        private int currentSelectedEffectIndex;
        private State state;

        public LedsMenu() : base(new Rgb(0xff, 0x00, 0x00))
        {
            state = State.SelectController;
            controllers = AesirCore.LightsCore.Leds.GetControllers().Select(k => k.Key).ToList();
            currentSelectedEffect = null;
            currentSelectedEffectIndex = -1;
            selectedEffectBackup = null;
            selectedController = Guid.Empty;
            new Thread(WorkerThread).Start();
        }

        private void WorkerThread()
        {
            while(true)
            {

                Thread.Sleep(100);
            }
        }

        private void SelectNextController(bool reverseOrder)
        {
            if(selectedController != Guid.Empty && selectedEffectBackup != null)
            {
                selectedEffectBackup.Resume();
                AesirCore.LightsCore.Leds.SetControllerEffect(selectedController, selectedEffectBackup, false);
                selectedEffectBackup = null;
            }

            
            if(selectedController == Guid.Empty)
            {
                selectedController = controllers.First();
            }
            else
            {
                int index = (controllers.IndexOf(selectedController) + (reverseOrder ? -1 : 1)) % controllers.Count;
                if (index < 0) index = controllers.Count - 1;

                selectedController = controllers[index];
            }

            SelectEffect effect = new SelectEffect();
            effect.SetBlinks(new Rgb(0, 0, 0xff), new Rgb(0, 127, 0xff));
            selectedEffectBackup = AesirCore.LightsCore.Leds.GetControllerEffect(selectedController);
            AesirCore.LightsCore.Leds.SetControllerEffect(selectedController, effect, false);
        }

        private void SelectNextEffect(bool reverseOrder)
        {
            if(currentSelectedEffect != null)
            {
                currentSelectedEffect.Dispose();
            }

            if (currentSelectedEffectIndex == -1)
                currentSelectedEffectIndex = 0;
            else
                currentSelectedEffectIndex = (currentSelectedEffectIndex + (reverseOrder ? -1 : 1)) % 4;

            if (currentSelectedEffectIndex < 0)
                currentSelectedEffectIndex = 3;

            switch(currentSelectedEffectIndex)
            {
                case 0:
                    currentSelectedEffect = new StaticEffect(new Rgb());
                    break;

                case 1:
                    currentSelectedEffect = new ColorCycleEffect();
                    break;

                case 2:
                    currentSelectedEffect = new RainbowEffect();
                    break;

                case 3:
                    currentSelectedEffect = new AudioEffect();
                    ((AudioEffect)currentSelectedEffect).SetBackgroundEffect(new ColorCycleEffect(), false);
                    break;
            }

            AesirCore.LightsCore.Leds.SetControllerEffect(selectedController, currentSelectedEffect, false);
        }


        public override void Reset()
        {
            base.Reset();
        }

        public override void OnTouch(float normalizedX, float normalizedY)
        {

            if (!Active) return;

            Rgb rgb = Rgb.FromHsv(normalizedX.Map(0, 1, 0, 359), normalizedY, 1);

            if (currentSelectedEffect == null || state != State.SelectEffectSettings) return;

            if(currentSelectedEffect is AudioEffect)
            {
                ((AudioEffect)currentSelectedEffect).SetBassColor(rgb);
            }
            else if(currentSelectedEffect is StaticEffect)
            {
                ((StaticEffect)currentSelectedEffect).SetStaticRgb(rgb);
            }

            AesirCore.LightsCore.Leds.SetControllerEffect(selectedController, currentSelectedEffect, false);

        }

        public override void OnButtonStateChange(ButtonType button, bool btnState)
        {
            if (!Active || !btnState) return;

            switch(button)
            {

                case ButtonType.Right:
                    switch (state)
                    {
                        case State.SelectController:
                            SelectNextController(false);
                            break;

                        case State.SelectEffect:
                            SelectNextEffect(false);
                            break;
                    }
                    break;

                case ButtonType.Left:

                    switch (state)
                    {
                        case State.SelectController:
                            SelectNextController(true);
                            break;

                        case State.SelectEffect:
                            SelectNextEffect(true);
                            break;
                    }
                    break;

                case ButtonType.Cross:
                    if (state == State.SelectController && selectedController != Guid.Empty)
                    {
                        if(selectedEffectBackup != null)
                        {
                            selectedEffectBackup.Dispose();
                            selectedEffectBackup = null;
                        }
                        state = State.SelectEffect;
                    }
                    else if (state == State.SelectEffect && currentSelectedEffect != null)
                        state = State.SelectEffectSettings;
                    else
                    {
                        state = State.SelectController;
                        
                    }
                    break;
            }
        }
    }
}
