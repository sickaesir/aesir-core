using CommonLib;
using DS4Library.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS4Library.State
{
    public class DS4Trigger
    {
        public class DS4TriggerEventArgs : EventArgs
        {
            public readonly float Push;

            public DS4TriggerEventArgs(float _push)
            {
                Push = _push;
            }

        }

        public float Push { get; private set; }
        public readonly TriggerType Type;
        public event EventHandler<DS4TriggerEventArgs> OnPush;
        public DS4Trigger(TriggerType trigger)
        {
            Type = trigger;
            OnPush = null;
        }

        public void ParseFromReport(byte[] report)
        {
            float push = report[Type == TriggerType.L2 ? 8 : 9];

            push = push.Map(0, 255, 0, 1);


            if(Push != push)
            {
                Push = push;
                OnPush?.Invoke(this, new DS4TriggerEventArgs(Push));
            }
        }
    }
}
