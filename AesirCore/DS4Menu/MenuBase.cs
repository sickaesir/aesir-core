
using CommonLib;
using DS4Library;
using DS4Library.State;
using DS4Library.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AesirCore.Menu
{
    public class MenuBase
    {
        public Rgb Color { get; private set; }
        public bool Active { get; set; } = false;
        public MenuBase(Rgb menuColor)
        {
            Color = menuColor;
        }

        public virtual void OnButtonStateChange(ButtonType button, bool state)
        {

        }

        //public virtual void OnGesture(GestureType gesture, byte touches)
        //{

        //}

        public virtual void OnStateUpdate(DS4State state)
        {

        }

        public virtual void Reset()
        {
            Active = false;
        }

        public virtual void OnTouch(float normalizedX, float normalizedY)
        {

        }

        public virtual void OnStick(StickType stick, float normalizedX, float normalizedY)
        {

        }
    }
}
