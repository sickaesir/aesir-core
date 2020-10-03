using AesirCore.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AesirCore.Aura
{
    public static class AuraIO
    {
        static object IOLocker;
        static IntPtr vgaHandle;
        static IntPtr moboHandle;


        #region Native Wrappers
        private static IntPtr GetVgaLedController()
        {
            lock(IOLocker)
            {
                uint controllersCount = AuraNativeIO.EnumerateGPU(null, 0);

                if (controllersCount == 0)
                    return IntPtr.Zero;

                IntPtr[] controllers = new IntPtr[controllersCount];
                AuraNativeIO.EnumerateGPU(controllers, controllersCount);

                return controllers.First();
            }
        }

        private static IntPtr GetMoboLedController()
        {
            lock (IOLocker)
            {
                uint controllersCount = AuraNativeIO.EnumerateMbController(null, 0);

                if (controllersCount == 0)
                    return IntPtr.Zero;

                IntPtr[] controllers = new IntPtr[controllersCount];
                AuraNativeIO.EnumerateMbController(controllers, controllersCount);

                return controllers.First();
            }
        }
        public static uint GetMotherBoardLedsCount()
        {
            lock (IOLocker)
            {
                return AuraNativeIO.GetMbLedCount(moboHandle);
            }
        }

        public static byte[] GetMotherBoardColors()
        {
            uint ledsCount = GetMotherBoardLedsCount();
            lock (IOLocker)
            {
                byte[] colors = new byte[ledsCount * 3];
                AuraNativeIO.GetMbColor(moboHandle, colors, (uint)colors.Length);
                return colors;
            }
        }
        public static uint SetMotherBoardColors(byte[] colors)
        {
            lock (IOLocker)
            {
                return AuraNativeIO.SetMbColor(moboHandle, colors, (uint)colors.Length);
            }
        }

        public static uint SetMotherBoardMode(uint mode)
        {
            lock (IOLocker)
            {
                return AuraNativeIO.SetMbMode(moboHandle, mode);
            }
        }

        public static uint GetGFXCardLedsCount()
        {
            lock (IOLocker)
            {
                return AuraNativeIO.GetGPULedCount(vgaHandle);
            }
        }

        public static uint SetGFXCardColors(byte[] colors)
        {
            lock (IOLocker)
            {
                return AuraNativeIO.SetGPUColor(vgaHandle, colors, (uint)colors.Length);
            }
        }

        public static uint SetGFXCardMode(uint mode)
        {
            lock (IOLocker)
            {
                return AuraNativeIO.SetGPUMode(vgaHandle, mode);
            }
        }
        #endregion

        public static IEnumerable<AuraDeviceController> CreateControllers()
        {
            List<AuraDeviceController> controllers = new List<AuraDeviceController>();

            for (uint i = 0; i < GetMotherBoardLedsCount(); i++)
                controllers.Add(new AuraDeviceController($"MotherBoard_{i + 1}", i, LedType.Motherboard));

            for (uint i = 0; i < GetGFXCardLedsCount(); i++)
                controllers.Add(new AuraDeviceController($"VGA_{i + 1}", i, LedType.Vga));

            return controllers;
        }


        public static void Init()
        {
            IOLocker = new object();
            vgaHandle = GetVgaLedController();
            moboHandle = GetMoboLedController();
        }
    }
}
