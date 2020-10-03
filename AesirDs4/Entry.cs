using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AesirDs4
{
    class AesirDS4
    {
        public static BluetoothDaemon BTDaemon { private set; get; }
        public static DS4Manager DSManager { private set; get; }
        public static LedsClient Leds { private set; get; }
        static void Main(string[] args)
        {
            DSManager = new DS4Manager();
            BTDaemon = new BluetoothDaemon();
            Leds = new LedsClient();

            Thread.Sleep(-1);
        }
    }
}
