using CommonLib;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AesirCore
{
    public class BluetoothDaemon
    {
        BluetoothEndPoint endpoint;
        BluetoothClient client;
        BluetoothComponent component;
        BluetoothWin32Authentication authenticator;
        ConcurrentQueue<BluetoothDeviceInfo> bluetoothDevices;
        ConcurrentDictionary<BluetoothAddress, bool> addressesToPair;
        
        public BluetoothDaemon()
        {
            bluetoothDevices = new ConcurrentQueue<BluetoothDeviceInfo>();
            addressesToPair = new ConcurrentDictionary<BluetoothAddress, bool>();
            endpoint = CreateEndpoint();

            if (endpoint == null) return;

            client = new BluetoothClient(endpoint);
            component = new BluetoothComponent(client);
            component.DiscoverDevicesComplete += OnDiscoverDevicesComplete;
            component.DiscoverDevicesProgress += OnDiscoverDevicesProgress;
            authenticator = new BluetoothWin32Authentication(OnBtAuthRequest);
            component.DiscoverDevicesAsync(255, true, true, true, true, null);


            new Thread(DaemonWorker).Start();
        }

        private void OnBtAuthRequest(object sender, BluetoothWin32AuthenticationEventArgs e)
        {

            if(addressesToPair.TryGetValue(e.Device.DeviceAddress, out bool authenticated) && !authenticated)
            {
                e.Confirm = true;
            }
        }

        private void OnDiscoverDevicesProgress(object sender, DiscoverDevicesEventArgs e)
        {
            foreach(var device in e.Devices)
            {
                if (bluetoothDevices.FirstOrDefault(k => k.DeviceAddress == device.DeviceAddress) != null) continue;

                Debug.WriteLine($"Discovered BT device, {device.DeviceName} ({device.DeviceAddress})");



                bluetoothDevices.Enqueue(device);
            }
        }

        private BluetoothEndPoint CreateEndpoint()
        {
            PhysicalAddress antennaMac = Helpers.GetBtAntennasMacs().FirstOrDefault();

            if (antennaMac == null)
                return null;

            BluetoothAddress address = BluetoothAddress.Parse(BitConverter.ToString(antennaMac.GetAddressBytes()).Replace("-", ":"));

            return new BluetoothEndPoint(address, BluetoothService.SerialPort);
        }

        private void DaemonWorker()
        {
            while(true)
            {
                ProcessDiscoveredDevices();

                Thread.Sleep(100);
            }
        }

        private void ProcessDiscoveredDevices()
        {
            while(bluetoothDevices.TryDequeue(out BluetoothDeviceInfo device))
            {
                device.Refresh();
                device.Update();

                if (device.ClassOfDevice.Value != 0x002508) continue;

                if (!device.Authenticated)
                {
                    addressesToPair.TryAdd(device.DeviceAddress, false);
                    if (BluetoothSecurity.PairRequest(device.DeviceAddress, null))
                    {
                        device.Refresh();
                        device.Update();
                        addressesToPair[device.DeviceAddress] = true;
                        Debug.WriteLine($"BT controller paired, {device.DeviceName} ({device.DeviceAddress})");
                        device.SetServiceState(BluetoothService.HumanInterfaceDevice, true, true);

                        //client.Connect(device.DeviceAddress, BluetoothService.SerialPort);
                        continue;
                    }
                    else
                    {
                        Debug.WriteLine($"BT controller pair fail, {device.DeviceName} ({device.DeviceAddress})");
                        continue;
                    }
                }
                else if (!device.Connected)
                {
                    // client.Connect(device.DeviceAddress, BluetoothService.SerialPort);
                    
                    //     device.SetServiceState(BluetoothService.HumanInterfaceDevice, true, true);
                    continue;
                }

               // else
               //     device.SetServiceState(BluetoothService.AudioSource, true, true);


            }
        }

        private void OnDiscoverDevicesComplete(object sender, DiscoverDevicesEventArgs e)
        {
            component.DiscoverDevicesAsync(255, true, true, true, true, null);
        }
    }
}
