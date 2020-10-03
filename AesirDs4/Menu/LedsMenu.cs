using CommonLib;
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
        private int selectedEditingController;
        private bool selected;
        public LedsMenu() : base(new Rgb(0xff, 0x00, 0x00))
        {
            selectedEditingController = -1;

            new Thread(WorkerThread).Start();
        }

        private void WorkerThread()
        {
            while(true)
            {
                if(selected)
                {
                    if(selectedEditingController == -1)
                    {
                        AesirDS4.Leds.SetAllLedsRgb(Rgb.FromHsv(normalizedX.Map(0, 1, 0, 359), 1, 1));
                    }
                }
                Thread.Sleep(100);
            }
        }

        public override void Reset()
        {
            base.Reset();
            selected = false;
            selectedEditingController = -2;
        }

        public override void OnTouch(float normalizedX, float normalizedY)
        {
        }

        public override void OnButtonStateChange(ButtonType button, bool state)
        {

        }
    }
}
