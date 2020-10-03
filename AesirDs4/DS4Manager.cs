using DS4Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AesirDs4
{
    public class DS4Manager
    {
        ConcurrentDictionary<string, Guid> mac2DeviceGuid;
        ConcurrentDictionary<Guid, AesirDS4Device> devices; 
        public DS4Manager()
        {
            mac2DeviceGuid = new ConcurrentDictionary<string, Guid>();
            devices = new ConcurrentDictionary<Guid, AesirDS4Device>();
            DS4DevicesManager.Init();
            DS4DevicesManager.OnDeviceAdded += OnDS4DeviceAdded;
            DS4DevicesManager.OnDeviceRemove += OnDS4DeviceRemoved;
        }

        private void OnDS4DeviceRemoved(object sender, EventArgs e)
        {
            DS4Device device = (DS4Device)sender;
            if (!mac2DeviceGuid.ContainsKey(device.Mac)) return;

            devices.TryRemove(mac2DeviceGuid[device.Mac], out var _);
            mac2DeviceGuid.TryRemove(device.Mac, out var _);
            Debug.WriteLine($"Disconnected DS4 {device.Mac}");
        }

        private void OnDS4DeviceAdded(object sender, EventArgs e)
        {
            DS4Device device = (DS4Device)sender;

            if (!mac2DeviceGuid.ContainsKey(device.Mac))
            {
                Guid deviceGuid = Guid.NewGuid();

                Debug.WriteLine($"Connected DS4 {device.Mac}");

                mac2DeviceGuid.TryAdd(device.Mac, deviceGuid);
                devices.TryAdd(deviceGuid, new AesirDS4Device(device));
            }
        }


    }
}
