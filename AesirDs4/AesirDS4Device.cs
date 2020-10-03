using AesirDs4.Menu;
using CommonLib;
using DS4Library;
using DS4Library.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AesirDs4
{
    class AesirDS4Device
    {

        DS4Device device;

        private readonly List<MenuBase> menus = new List<MenuBase>()
        {
            new MenuBase(new Rgb()) { Active = true },
            new LedsMenu(),
            new MusicMenu()
        };

        private int selectedMenuIndex;

        // f8da0cc86adc
        public AesirDS4Device(DS4Device _device)
        {
            selectedMenuIndex = 0;
            device = _device;
            InitMenus();

        }

        private void InitMenus()
        {
            foreach(var menu in menus)
            {
                foreach(var button in device.ControllerState.GetButtons())
                {
                    button.OnPress += (e, b) =>
                    {
                        DS4Button btn = (DS4Button)e;
                        menu.OnButtonStateChange(btn.Type, true);
                    };

                    button.OnRelease += (e, b) =>
                    {
                        DS4Button btn = (DS4Button)e;
                        menu.OnButtonStateChange(btn.Type, false);
                    };
                }

                foreach(var stick in device.ControllerState.GetSticks())
                {
                    stick.OnStickMoved += (e, b) =>
                    {
                        DS4Stick s = (DS4Stick)e;
                        menu.OnStick(s.Type, b.X, b.Y);
                    };
                }

                device.OnDS4ReportReceived += (a, b) =>
                {
                    menu.OnStateUpdate(device.ControllerState);
                };

                device.ControllerState.TouchPad.TouchMove += (a, b) =>
                {
                    menu.OnTouch(b.Touch.X, b.Touch.Y);
                };
            }

            device.ControllerState.GetButton(Structs.ButtonType.PS).OnPress += (a, b) => OnChangeMenu();
        }

        private void OnChangeMenu()
        {
            int oldMenuIndex = selectedMenuIndex;
            int newMenuIndex = (selectedMenuIndex + 1) % menus.Count;

            menus[oldMenuIndex].Reset();
            menus[newMenuIndex].Active = true;
            device.LightBarColor = menus[newMenuIndex].Color;

            selectedMenuIndex = newMenuIndex;
        }

        private void OnDeviceReport(object sender, EventArgs e)
        {
            MenuBase menu = menus[selectedMenuIndex];





        }
    }
}
