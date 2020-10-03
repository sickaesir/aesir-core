using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS4Library.State
{
    public class DS4Touch
    {
        public float X { private set; get; }
        public float Y { private set; get; }
        public byte Id { private set; get; }
        public bool Active { private set; get; }

        public EventHandler<DS4TouchPad.TouchEventArgs> OnMove;
        public DS4Touch()
        {
            OnMove = null;
        }

        internal void ParseFromReport(byte[] report, int offset)
        {
            Active = (report[offset] >> 7) != 0 ? false : true;
            Id = (byte)(report[offset] & 0x7f);
            float x = (report[offset + 1] + ((report[offset + 2] & 0x0F) * 255));
            float y = (((report[offset + 2] & 0xF0) >> 4) + (report[offset + 3] * 16));

            x = x.Map(0, 1920, 0, 1);
            y = y.Map(0, 1080, 0, 1);

            if(X != x || y != Y)
            {
                X = x;
                Y = y;
                OnMove?.Invoke(this, new DS4TouchPad.TouchEventArgs(this));
            }
        }
    }
}
