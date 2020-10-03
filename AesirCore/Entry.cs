using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using AesirLights;
using AesirLights.Structs;

namespace AesirCore
{
    public static class AesirCore
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        public static Core LightsCore { get; private set; }
        public static DS4Manager ControllersManager { get; private set; }
        public static BluetoothDaemon BTDaemon { get; private set; }
        public static void Main(string[] args)
        {
            //Console.WindowWidth = 46;
            //Console.BufferWidth = 46;
            //Console.WindowHeight = 47;
            //Console.BufferHeight = 47;

#if !DEBUG
            ShowWindow(GetConsoleWindow(), SW_HIDE);
#endif

            LightsCore = new Core();
            BTDaemon = new BluetoothDaemon();
            ControllersManager = new DS4Manager();
            Thread.Sleep(-1);
        }
    }
}