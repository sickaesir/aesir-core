using DS4Library.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS4Library.State
{
    public class DS4Button
    {
        public bool State { private set; get; }
        public readonly ButtonType Type;
        public event EventHandler OnPress;
        public event EventHandler OnRelease;
        public DS4Button(ButtonType _type)
        {
            Type = _type;
            State = false;
            OnPress = null;
            OnRelease = null;
        }

        internal void SetButtonState(bool newState)
        {
            bool oldState = State;
            State = newState;

            if (oldState == false && newState == true)
                OnPress?.Invoke(this, new EventArgs());
            else if (oldState == true && newState == false)
                OnRelease?.Invoke(this, new EventArgs());
        }
    }
}
