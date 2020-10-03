using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS4Library.State
{
    public class DS4Accelerometer
    {
        public class AccelerometerEventArgs : EventArgs
        {
            public readonly float X;
            public readonly float Y;
            public readonly float Z;

            public AccelerometerEventArgs(float _x, float _y, float _z)
            {
                X = _x;
                Y = _y;
                Z = _z;
            }
        }

        public float X { private set; get; }
        public float Y { private set; get; }
        public float Z { private set; get; }


        public event EventHandler<AccelerometerEventArgs> OnAccelerometerMove;

        public DS4Accelerometer()
        {
            OnAccelerometerMove = null;
        }

        internal void ParseFromReport(byte[] report)
        {
            float x = ((report[14] << 8) | report[15]);
            float y = ((report[16] << 8) | report[17]);
            float z = ((report[18] << 8) | report[19]);

            x = x.Map(-32768, 32767, -1, 1);
            y = y.Map(-32768, 32767, -1, 1);
            z = z.Map(-32768, 32767, -1, 1);


            if (x != X || y != Y || z != Z)
            {
                X = x;
                Y = y;
                Z = z;

                OnAccelerometerMove?.Invoke(this, new AccelerometerEventArgs(X, Y, Z));
            }
        }


    }
}
