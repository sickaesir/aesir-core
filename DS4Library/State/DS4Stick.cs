using CommonLib;
using DS4Library.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS4Library.State
{
    public class DS4Stick
    {
        public class DS4StickEventArgs : EventArgs
        {
            public readonly float X;
            public readonly float Y;
            
            public DS4StickEventArgs(float _x, float _y)
            {
                X = _x;
                Y = _y;
            }
        }
        public readonly StickType Type;
        public event EventHandler<DS4StickEventArgs> OnStickMoved;


        public float X { private set; get; }
        public float Y { private set; get; }
        
        public DS4Stick(StickType stick)
        {
            Type = stick;
            OnStickMoved = null;
        }

        public void ParseFromReport(byte[] report)
        {
            float _X = ((float)report[Type == StickType.L ? 1 : 3]).Map(0, 255, -1, 1);
            float _Y = ((float)report[Type == StickType.L ? 2 : 4]).Map(0, 255, -1, 1);


            if(_X != X || _Y != Y)
            {

                X = _X;
                Y = _Y;
                OnStickMoved?.Invoke(this, new DS4StickEventArgs(_X, _Y));
            }
        }
    }
}
