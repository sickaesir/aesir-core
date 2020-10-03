using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Threading.Tasks;


using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Drawing;
using CommonLib;
using HidLibrary;
using System.Media;
using DS4Library.State;

namespace DS4Library
{
    public class DS4Device
    {
        public HidDevice DS4HidDevice { get; private set; }
        public string Mac { get; private set; }
        public DS4ConnectionType.Type ConnectionType { get; private set; }

        public event EventHandler<EventArgs> OnDS4ReportReceived = null;
        public event EventHandler<EventArgs> OnDS4DeviceRemoval = null;

        public DS4State ControllerState { private set; get; }

        public int ReadLatency { get => readLatencies.Count() == 0 ? 0 : (int)readLatencies.Average(); }
        public int WriteLatency { get => writeLatencies.Count() == 0 ? 0 : (int)writeLatencies.Average(); }
        public int Latency { get => (int)(ReadLatency + WriteLatency) / 2; }
        public int IOTimeout { get => readLatencies.Count() > 1 ? DS4Constants.DS4_IO_TIMEOUT_MS : DS4Constants.DS4_IO_HANDSHAKE_TIMEOUT_MS; }

        public Rgb LightBarColor { get; set; }

        private float _lightBarOnDuration;
        public float LigthBarOnDuration { get => _lightBarOnDuration; set { _lightBarOnDuration = value.Clamp(0, 1); } }

        private float _lightBarOffDuration;
        public float LightBarOffDuration { get => _lightBarOffDuration; set { _lightBarOffDuration = value.Clamp(0, 1); } }

        private float _softRumble;
        public float SoftRumble { get => _softRumble; set { _softRumble = value.Clamp(0, 1); } }

        private float _hardRumble;
        public float HardRumble { get => _hardRumble; set { _hardRumble = value.Clamp(0, 1); } }

        private byte _pollRate;
        public byte PollRate { get => _pollRate; set { _pollRate = value.Clamp((byte)0x0, (byte)0x10); } }

        // private vars
        private bool readWorkerThreadEnabled;
        private Thread readWorkerThread;
        private List<long> readLatencies;

        private bool writeWorkerThreadEnabled;
        private Thread writeWorkerThread;
        private List<long> writeLatencies;


        public DS4Device(HidDevice _hidDevice)
        {
            DS4HidDevice = _hidDevice;
            ConnectionType = DS4ConnectionType.GetConnectionType(DS4HidDevice);

            Mac = DS4HidDevice.ReadSerial();


            LightBarColor = new Rgb();

            ControllerState = new DS4State();

            readWorkerThread = null;
            readWorkerThreadEnabled = false;

            writeWorkerThread = null;
            writeWorkerThreadEnabled = false;
            readLatencies = new List<long>();
            writeLatencies = new List<long>();

            StartReadWorkerThread();
            StartWriteWorkerThread();
        }

        private bool SendData(byte[] data)
        {
            switch (ConnectionType)
            {
                case DS4ConnectionType.Type.Bluetooth:
                    return DS4HidDevice.WriteOutputReportViaControl(data);

                case DS4ConnectionType.Type.USB:
                    return DS4HidDevice.WriteOutputReportViaInterrupt(data, 8);

                default:
                    return false;

            }
        }

        private void OnHidTimeout(object sender)
        {
            DS4HidDevice.CancelIO();
        }

        private HidDevice.ReadStatus ReadData(out byte[] data)
        {
            data = null;


            switch (ConnectionType)
            {
                case DS4ConnectionType.Type.Bluetooth:
                    {
                        data = new byte[DS4Constants.DS4_BT_INPUT_REPORT_LENGTH];
                    }
                    break;

                case DS4ConnectionType.Type.USB:
                    {
                        data = new byte[DS4Constants.DS4_USB_INPUT_REPORT_LENGTH];
                    }
                    break;


                default:
                    return HidDevice.ReadStatus.ReadError;
            }

            HidDevice.ReadStatus status = HidDevice.ReadStatus.ReadError;
            using (var timeoutTimer = new System.Timers.Timer(IOTimeout))
            {
                timeoutTimer.Elapsed += OnHidReadTimeout;
                timeoutTimer.Enabled = true;
                status = DS4HidDevice.ReadFile(data);
                timeoutTimer.Enabled = false;
            }

            return status;
        }

        private void OnHidReadTimeout(object sender, System.Timers.ElapsedEventArgs e)
        {
            DS4HidDevice.CancelIO();
        }

        private void StartReadWorkerThread()
        {
            readLatencies.Clear();
            readLatencies.Add(0);

            if (readWorkerThreadEnabled) return;

            readWorkerThreadEnabled = true;
            readWorkerThread = new Thread(ReadWorkerThread);
            readWorkerThread.Start();
        }

        private void StartWriteWorkerThread()
        {
            writeLatencies.Clear();
            writeLatencies.Add(0);

            if (writeWorkerThreadEnabled) return;

            writeWorkerThreadEnabled = true;
            writeWorkerThread = new Thread(WriteWorkerThread);
            writeWorkerThread.Start();
        }

         private void StopReadWorkerThread()
        {
            if (!readWorkerThreadEnabled) return;

            readWorkerThreadEnabled = false;

            readLatencies.Clear();
        }

        private void StopWriteWorkerThread()
        {
            if (!writeWorkerThreadEnabled) return;

            writeWorkerThreadEnabled = false;

            writeLatencies.Clear();
        }


        private void WriteWorkerThread()
        {
            try
            {
                Stopwatch elapsedStopwatch = new Stopwatch();
                elapsedStopwatch.Start();
                while (writeWorkerThreadEnabled)
                {
                    writeLatencies.Add(elapsedStopwatch.ElapsedMilliseconds - writeLatencies.Last());

                    if (writeLatencies.Count > 100)
                        writeLatencies.RemoveAt(0);

                    WriteWorkerTick();

                    Thread.Sleep(100);
                }
            }
            catch(ThreadAbortException)
            {

            }
        }

        private byte[] CreateOutputReport()
        {
            byte[] output = null;
            switch (ConnectionType)
            {
            
                case DS4ConnectionType.Type.Bluetooth:
                    output = new byte[DS4Constants.DS4_BT_OUTPUT_REPORT_LENGTH];

                    output[0] = 0x11;
                    output[1] = (byte)(0x80 | PollRate);
                    output[3] = 0xff;

                    output[6] = (byte)(SoftRumble * 255);
                    output[7] = (byte)(HardRumble * 255);

                    output[8] = LightBarColor.r;
                    output[9] = LightBarColor.g;
                    output[10] = LightBarColor.b;

                    output[11] = (byte)(LigthBarOnDuration * 255);
                    output[12] = (byte)(LightBarOffDuration * 255);
                    break;

                case DS4ConnectionType.Type.USB:
                    output = new byte[DS4HidDevice.Capabilities.OutputReportByteLength];
                    output[0] = 0x05;
                    output[1] = (byte)(0x80 | PollRate);
                    output[4] = (byte)(SoftRumble * 255);
                    output[5] = (byte)(HardRumble * 255);
                    output[6] = LightBarColor.r;
                    output[7] = LightBarColor.g;
                    output[8] = LightBarColor.b;
                    output[9] = (byte)(LigthBarOnDuration * 255);
                    output[10] = (byte)(LightBarOffDuration * 255);
                    break;
            }


            return output;
        }

        private void WriteWorkerTick()
        {
            if(!SendData(CreateOutputReport()))
            {
                return;
            }
            else
            {
            }
        }

        private void ReadWorkerTick()
        {
            byte[] readData;
            HidDevice.ReadStatus read = ReadData(out readData);

            if (read != HidDevice.ReadStatus.Success)
            {
                Shutdown();
                return;
            }

            switch(readData[0])
            {
                case 1:

                    break;

                case 0x11:
                    ControllerState.ParseFromReport(readData.Skip(ConnectionType == DS4ConnectionType.Type.Bluetooth ? 2 : 0).ToArray());
                    OnDS4ReportReceived?.Invoke(this, new EventArgs());
                    break;

            }


        }

        public void Shutdown()
        {
            StopReadWorkerThread();
            StopWriteWorkerThread();

            if (ConnectionType == DS4ConnectionType.Type.Bluetooth && Mac != null)
            {
                IntPtr btHandle = IntPtr.Zero;
                int IOCTL_BTH_DISCONNECT_DEVICE = 0x41000c;

                byte[] btAddr = new byte[8];
                string[] sbytes = Mac.Split(':');
                for (int i = 0; i < 6; i++)
                {
                    btAddr[5 - i] = Convert.ToByte(sbytes[i], 16);
                }
                long lbtAddr = BitConverter.ToInt64(btAddr, 0);

                NativeMethods.BLUETOOTH_FIND_RADIO_PARAMS p = new NativeMethods.BLUETOOTH_FIND_RADIO_PARAMS();
                p.dwSize = Marshal.SizeOf(typeof(NativeMethods.BLUETOOTH_FIND_RADIO_PARAMS));
                IntPtr searchHandle = NativeMethods.BluetoothFindFirstRadio(ref p, ref btHandle);
                int bytesReturned = 0;
                bool success = false;
                while (!success && btHandle != IntPtr.Zero)
                {
                    success = NativeMethods.DeviceIoControl(btHandle, IOCTL_BTH_DISCONNECT_DEVICE, ref lbtAddr, 8, IntPtr.Zero, 0, ref bytesReturned, IntPtr.Zero);
                    NativeMethods.CloseHandle(btHandle);
                    if (!success)
                        if (!NativeMethods.BluetoothFindNextRadio(searchHandle, ref btHandle))
                            btHandle = IntPtr.Zero;

                }
                NativeMethods.BluetoothFindRadioClose(searchHandle);
            }

            OnDS4DeviceRemoval?.Invoke(this, new EventArgs());
        }

        private void ReadWorkerThread()
        {
            try
            {
                Stopwatch elapsedStopwatch = new Stopwatch();
                elapsedStopwatch.Start();
                while (readWorkerThreadEnabled)
                {
                    readLatencies.Add(elapsedStopwatch.ElapsedMilliseconds - readLatencies.Last());

                    if (readLatencies.Count > 100)
                        readLatencies.RemoveAt(0);


                    ReadWorkerTick();
                }
            }
            catch(ThreadAbortException)
            {

            }
        }

        public bool ReadDS4ConnectAddress(ref byte[] bluetoothAddress)
        {
            if (ConnectionType != DS4ConnectionType.Type.USB) return false;

            bluetoothAddress = null;
            byte[] data = new byte[DS4HidDevice.Capabilities.InputReportByteLength];
            data[0] = 0x12;

            if (!DS4HidDevice.ReadFeatureData(data))
                return false;

            bluetoothAddress = new byte[6];
            for (int i = 15, w = 0; i > 15 - 6; i--, w++)
                bluetoothAddress[w] = data[i];

            return true;
            
        }

        public bool WriteDS4ConnectAddress(byte[] bluetoothAddress)
        {
            if (ConnectionType != DS4ConnectionType.Type.USB) return false;


            byte[] data = new byte[] {
                0x13, 
                bluetoothAddress[5], 
                bluetoothAddress[4], 
                bluetoothAddress[3], 
                bluetoothAddress[2], 
                bluetoothAddress[1], 
                bluetoothAddress[0], 
                0x56, 0xE8, 0x81, 0x38, 0x08, 0x06, 0x51, 0x41, 0xC0, 0x7F, 0x12, 0xAA, 0xD9, 0x66, 0x3C, 0xCE
            };

            return DS4HidDevice.WriteFeatureData(data);
        }

        public override string ToString()
        {
            string str = $"DualShock 4 - {Mac}\n";
            str += $"\tPower Source - {(ControllerState.Charging ? "Cable" : "Battery")}\n";
            str += $"\tBattery Level - {(ControllerState.BatteryLevel == 0 ? "Charging" : ControllerState.BatteryLevel + "%")}\n";
            str += $"\tColor - R:{LightBarColor.r} G:{LightBarColor.g} B:{LightBarColor.b}\n";
            str += $"\tVibration - Hard: {HardRumble} Soft: {SoftRumble}\n";
            str += $"\tAccelerometer - X: {ControllerState.Accelerometer.X} Y: {ControllerState.Accelerometer.Y} Z: {ControllerState.Accelerometer.Z}\n";
            str += $"\tGyroscope - Yaw: {ControllerState.Gyroscope.Yaw} Pitch: {ControllerState.Gyroscope.Pitch} Roll: {ControllerState.Gyroscope.Roll}\n";
            str += $"\n\tTriggers\n";

            foreach(var trigger in ControllerState.GetTriggers())
            {
                str += $"\t\t{trigger.Type} - Push: {trigger.Push}\n";
            }

            str += $"\n\tSticks\n";

            foreach(var stick in ControllerState.GetSticks())
            {
                str += $"\t\t{stick.Type} - X: {stick.X} Y: {stick.Y}\n";
            }

            str += $"\n\tTouchPad\n";

            foreach(var touch in ControllerState.TouchPad.GetCurrentTouches())
            {
                str += $"\t\tTouch - Id: {touch.Id} Active: {touch.Active} X: {touch.X} Y: {touch.Y}\n";
            }

            str += $"\n\tButtons\n";

            foreach(var button in ControllerState.GetButtons())
            {
                str += $"\t\t{button.Type} - State: {(button.State ? "Pressed" : "Released")}\n";
            }
            

            return str;
        }
    }
}
