using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS4Library.State
{
    public class DS4TouchPad
    {
        public class TouchEventArgs : EventArgs
        {
            public readonly DS4Touch Touch;

            public TouchEventArgs(DS4Touch touch)
            {
                Touch = touch;
            }
        }
        private Dictionary<byte, DS4Touch> activeTouches;

        public event EventHandler<TouchEventArgs> TouchStart;
        public event EventHandler<TouchEventArgs> TouchEnd;
        public event EventHandler<TouchEventArgs> TouchMove;

        public DS4TouchPad()
        {
            TouchStart = null;
            TouchEnd = null;
            TouchMove = null;
            activeTouches = new Dictionary<byte, DS4Touch>();
        }

        public IEnumerable<DS4Touch> GetCurrentTouches()
        {
            return activeTouches.Values;
        }

        public void ParseFromReport(byte[] report)
        {
            for(byte i = 0; i < report[33]; i++)
            {
                int packetCounter = report[34 + (i * 9)];

                for(int k = 0; k < DS4Constants.DS4_TOUCHPAD_TOUCH_CAPACITY; k++)
                {
                    bool active = (report[35 + (i * 9) + (k * 4)] >> 7) != 0 ? false : true;
                    byte id = (byte)(report[35 + (i * 9) + (k * 4)] & 0x7f);

                    if (active && !activeTouches.ContainsKey(id))
                    {
                        DS4Touch newTouch = new DS4Touch();

                        newTouch.OnMove += TouchMove;

                        activeTouches.Add(id, newTouch);
                        TouchStart?.Invoke(activeTouches[id], new TouchEventArgs(activeTouches[id]));
                    }

                    if (activeTouches.ContainsKey(id))
                        activeTouches[id].ParseFromReport(report, 35 + (i * 9) + (k * 4));

                    if (!active && activeTouches.ContainsKey(id))
                    {
                        DS4Touch touch = activeTouches[id];
                        TouchEnd?.Invoke(touch, new TouchEventArgs(touch));
                        activeTouches.Remove(id);
                    }
                }


            }

        }
    }
}
