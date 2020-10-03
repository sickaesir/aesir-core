using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS4Library.State
{
    public class DS4Gyroscope
    {

        public class GyroscopeEventArgs : EventArgs
        {
            public readonly short Yaw;
            public readonly short Pitch;
            public readonly short Roll;

            public GyroscopeEventArgs(short _yaw, short _pitch, short _roll)
            {
                Yaw = _yaw;
                Pitch = _pitch;
                Roll = _roll;
            }
        }

        public short Yaw { get; private set; }
        public short Pitch { get; private set; }
        public short Roll { get; private set; }

        public event EventHandler<GyroscopeEventArgs> OnGyroscopeMove;

        public DS4Gyroscope()
        {
            OnGyroscopeMove = null;
        }

        public void ParseFromReport(byte[] report)
        {

            short yaw = (short)((report[20] << 8) | report[21]);
            short pitch = (short)((report[22] << 8) | report[23]);
            short roll = (short)((report[24] << 8) | report[25]);

            if (yaw != Yaw || pitch != Pitch || roll != Roll)
            {
                Yaw = yaw;
                Pitch = pitch;
                Roll = roll;
                OnGyroscopeMove?.Invoke(this, new GyroscopeEventArgs(Yaw, Pitch, Roll));
            }
        }
    }
}
