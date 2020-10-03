using AesirCore.Controllers;
using AesirCore.Effects;
using AesirLights;
using AesirLights.Controllers;
using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AesirCore
{
    public class LedsManager
    {
        private Dictionary<Guid, DeviceController> Controllers;
        private Dictionary<Guid, Type> Effects;
        private CorsairHubManager corsairHubManager;
        public LedsManager()
        {
            Controllers = new Dictionary<Guid, DeviceController>();
            corsairHubManager = new CorsairHubManager();
            InitControllers();
            InitEffects();

        }

        private void InitControllers()
        {

            // mouse
            AddController(new MouseDeviceController());

            // corsair rgb controller
            foreach (CorsairHubController controller in corsairHubManager.GetControllers())
                AddController(controller);

            // mobo & vgas
            foreach (AuraDeviceController controller in AuraSyncSharp.Controllers)
                AddController(controller);

            // keyboard
            AddController(new KeyboardDeviceController());

            // audio interface
            {
                var controller = AddController(new ScarlettDeviceController());

                var effect = new TransitionEffect(new Rgb(0, 0, 0), new Rgb(6, 0, 0));
                effect.TransitionEnded += (s, e) =>
                {
                    effect.SetPercentage(0);
                };

                controller.SetEffect(effect, false);
            }

            // ds4
            //DS4Windows.DS4Devices.findControllers();
            //var ds4s = DS4Windows.DS4Devices.getDS4Controllers();


            foreach (var controller in Controllers)
                controller.Value.Initialize();
        }


        private void InitEffects()
        {
            Effects = new Dictionary<Guid, Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Type[] assemblyTypes = assembly.GetTypes();

                foreach (Type assemblyType in assemblyTypes)
                {
                    if (assemblyType.BaseType != typeof(EffectBase)) continue;

                    Effects.Add(Guid.NewGuid(), assemblyType);
                }
            }
        }

        public T CreateEffectFromGuid<T>(Guid guid) where T : class
        {
            if (!Effects.ContainsKey(guid)) return null;

            Type effectType = Effects[guid];

            return (T)Activator.CreateInstance(effectType);
        }

        public DeviceController AddController(DeviceController controller)
        {
            Guid deviceGuid = Guid.NewGuid();

            Controllers.Add(deviceGuid, controller);

            return controller;
        }

        public List<KeyValuePair<Guid, string>> GetEffects()
        {
            List<KeyValuePair<Guid, string>> effects = new List<KeyValuePair<Guid, string>>();

            foreach (KeyValuePair<Guid, Type> effectTypes in Effects)
            {
                effects.Add(new KeyValuePair<Guid, string>(effectTypes.Key, effectTypes.Value.Name.Replace("Effect", "")));
            }

            return effects;
        }

        public List<KeyValuePair<Guid, string>> GetControllers()
        {
            List<KeyValuePair<Guid, string>> outControllers = new List<KeyValuePair<Guid, string>>();

            foreach (var controller in Controllers)
                outControllers.Add(new KeyValuePair<Guid, string>(controller.Key, controller.Value.Name));

            return outControllers;
        }

        public EffectBase GetControllerEffect(Guid controller)
        {
            if (!Controllers.ContainsKey(controller)) return null;

            return Controllers[controller].Effect;
        }

        public void SetControllerEffectOnAllControllers(EffectBase effect, bool transition = true)
        {
            foreach (var controller in Controllers)
                SetControllerEffect(controller.Key, effect, transition);
        }

        public bool SetControllerEffect(Guid guid, EffectBase effect, bool transition = true)
        {
            if (!Controllers.ContainsKey(guid)) return false; 

            Controllers[guid].SetEffect(effect, transition);



            return true;
        }

        public bool SetControllerBrightness(Guid guid, int brightness)
        {
            if (!Controllers.ContainsKey(guid)) return false;

            Controllers[guid].SetBrightness(brightness);

            return true;
        }

        public void SetControllerBrightnessOnAllControllers(int brightness)
        {
            foreach (var controller in Controllers)
                SetControllerBrightness(controller.Key, brightness);
        }

        public void PrintControllers()

        {
            foreach (var controller in Controllers)
            {
                controller.Value.PrintControllerLine();
            }
        }

    }
}
