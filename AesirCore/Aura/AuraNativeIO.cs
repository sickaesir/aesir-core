using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AesirCore.Aura
{
    public static class AuraNativeIO
    {
        #region SDK Imports
        [DllImport("AURA_SDK.dll")]
        public extern static uint EnumerateMbController([Out] [In] IntPtr[] handles, uint size);

        [DllImport("AURA_SDK.dll")]
        public extern static uint SetMbMode(IntPtr handle, uint mode);

        [DllImport("AURA_SDK.dll")]
        public extern static uint SetMbColor(IntPtr handle, byte[] color, uint size);

        [DllImport("AURA_SDK.dll")]
        public extern static uint GetMbColor(IntPtr handle, [Out] byte[] color, uint size);

        [DllImport("AURA_SDK.dll")]
        public extern static uint GetMbLedCount(IntPtr handle);

        [DllImport("AURA_SDK.dll")]
        public extern static uint EnumerateGPU([In] [Out] IntPtr[] handles, uint size);

        [DllImport("AURA_SDK.dll")]
        public extern static uint SetGPUMode(IntPtr handle, uint mode);

        [DllImport("AURA_SDK.dll")]
        public extern static uint SetGPUColor(IntPtr handle, byte[] color, uint size);

        [DllImport("AURA_SDK.dll")]
        public extern static uint GetGPULedCount(IntPtr handle);
        #endregion
    }
}
