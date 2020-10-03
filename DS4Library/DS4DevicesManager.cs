
using HidLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

namespace DS4Library
{
    public static class DS4DevicesManager
    {
        private static Dictionary<string, DS4Device> Devices = new Dictionary<string, DS4Device>();

        public static event EventHandler OnDeviceRemove;
        public static event EventHandler OnDeviceAdded;

        public static void Init()
        {
            OnDeviceAdded = null;
            OnDeviceAdded = null;
            new Thread(WorkerThread).Start();
        }

        private static void WorkerThread()
        {
            while(true)
            {
                RefreshControllers();
                Thread.Sleep(100);
            }
        }

        //enumerates ds4 controllers in the system
        private static void RefreshControllers()
        {
            lock (Devices)
            {
                int[] pid = { 0xBA0, 0x5C4, 0x09CC };

                IEnumerable<HidDevice> hDevices = HidDevices.Enumerate(0x054C, pid);

                // Sort Bluetooth first in case USB is also connected on the same controller.
                hDevices = hDevices.OrderBy<HidDevice, DS4ConnectionType.Type>((HidDevice d) => { return DS4ConnectionType.GetConnectionType(d); });

                foreach (HidDevice hDevice in hDevices)
                {
                    if (Devices.ContainsKey(hDevice.DevicePath))
                        continue; // BT/USB endpoint already open once
                    if (!hDevice.IsOpen)
                    {
                        hDevice.OpenDevice(false);
                    }

                    if (hDevice.IsOpen)
                    {
                        if (Devices.ContainsKey(hDevice.DevicePath))
                            continue; // happens when the BT endpoint already is open and the USB is plugged into the same host
                        else
                        {
                            DS4Device ds4Device = new DS4Device(hDevice);
                            ds4Device.OnDS4DeviceRemoval += OnDeviceRemoved;
                            Devices.Add(hDevice.DevicePath, ds4Device);
                            OnDeviceAdded?.Invoke(ds4Device, new EventArgs());
                        }
                    }
                }
                
            }
        }

        private static void OnDeviceRemoved(object sender, EventArgs e)
        {
            lock (Devices)
            {
                DS4Device device = (DS4Device)sender;
                device.DS4HidDevice.CloseDevice();
                Devices.Remove(device.DS4HidDevice.DevicePath);
                OnDeviceRemove?.Invoke(device, new EventArgs());
            }
        }

    }
}
