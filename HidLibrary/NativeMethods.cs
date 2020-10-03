﻿using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles; 
namespace HidLibrary
{
    public static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct BLUETOOTH_FIND_RADIO_PARAMS
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwSize;
        }

        [DllImport("bthprops.cpl", CharSet = CharSet.Auto)]
        public extern static IntPtr BluetoothFindFirstRadio(ref BLUETOOTH_FIND_RADIO_PARAMS pbtfrp, ref IntPtr phRadio);

        [DllImport("bthprops.cpl", CharSet = CharSet.Auto)]
        public extern static bool BluetoothFindNextRadio(IntPtr hFind, ref IntPtr phRadio);

        [DllImport("bthprops.cpl", CharSet = CharSet.Auto)]
        public extern static bool BluetoothFindRadioClose(IntPtr hFind);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Boolean DeviceIoControl(IntPtr DeviceHandle, Int32 IoControlCode, ref long InBuffer, Int32 InBufferSize, IntPtr OutBuffer, Int32 OutBufferSize, ref Int32 BytesReturned, IntPtr Overlapped);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr hObject);

	    public const int FILE_FLAG_OVERLAPPED = 0x40000000;
	    public const short FILE_SHARE_READ = 0x1;
	    public const short FILE_SHARE_WRITE = 0x2;
	    public const uint GENERIC_READ = 0x80000000;
	    public const uint GENERIC_WRITE = 0x40000000;
        public const Int32 FileShareRead = 1;
        public const Int32 FileShareWrite = 2;
        public const Int32 OpenExisting = 3;
	    public const int ACCESS_NONE = 0;
	    public const int INVALID_HANDLE_VALUE = -1;
	    public const short OPEN_EXISTING = 3;
	    public const int WAIT_TIMEOUT = 0x102;
	    public const uint WAIT_OBJECT_0 = 0;
	    public const uint WAIT_FAILED = 0xffffffff;

	    public const int WAIT_INFINITE = 0xffff;
	    [StructLayout(LayoutKind.Sequential)]
	    public struct OVERLAPPED
	    {
		    public int Internal;
		    public int publicHigh;
		    public int Offset;
		    public int OffsetHigh;
		    public int hEvent;
	    }

	    [StructLayout(LayoutKind.Sequential)]
	    public struct SECURITY_ATTRIBUTES
	    {
		    public int nLength;
		    public IntPtr lpSecurityDescriptor;
		    public bool bInheritHandle;
	    }

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        static public extern bool CancelIo(IntPtr hFile);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        static public extern bool CancelIoEx(IntPtr hFile, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        static public extern bool CancelSynchronousIo(IntPtr hObject);

	    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	    static public extern IntPtr CreateEvent(ref SECURITY_ATTRIBUTES securityAttributes, int bManualReset, int bInitialState, string lpName);

	    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	    static public extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, ref SECURITY_ATTRIBUTES lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, int hTemplateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(String lpFileName, UInt32 dwDesiredAccess, Int32 dwShareMode, IntPtr lpSecurityAttributes, Int32 dwCreationDisposition, Int32 dwFlagsAndAttributes, Int32 hTemplateFile);
        [DllImport("kernel32.dll", SetLastError = true)]
        static public extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

	    [DllImport("kernel32.dll")]
	    static public extern uint WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

        [DllImport("kernel32.dll")]
        static public extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

	    public const int DBT_DEVICEARRIVAL = 0x8000;
	    public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
	    public const int DBT_DEVTYP_DEVICEINTERFACE = 5;
	    public const int DBT_DEVTYP_HANDLE = 6;
	    public const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
	    public const int DEVICE_NOTIFY_SERVICE_HANDLE = 1;
	    public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;
	    public const int WM_DEVICECHANGE = 0x219;
	    public const short DIGCF_PRESENT = 0x2;
	    public const short DIGCF_DEVICEINTERFACE = 0x10;
	    public const int DIGCF_ALLCLASSES = 0x4;
        public const int DICS_ENABLE = 1;
        public const int DICS_DISABLE = 2;
        public const int DICS_FLAG_GLOBAL = 1;
        public const int DIF_PROPERTYCHANGE = 0x12;

        public const int MAX_DEV_LEN = 1000;
	    public const int SPDRP_ADDRESS = 0x1c;
	    public const int SPDRP_BUSNUMBER = 0x15;
	    public const int SPDRP_BUSTYPEGUID = 0x13;
	    public const int SPDRP_CAPABILITIES = 0xf;
	    public const int SPDRP_CHARACTERISTICS = 0x1b;
	    public const int SPDRP_CLASS = 7;
	    public const int SPDRP_CLASSGUID = 8;
	    public const int SPDRP_COMPATIBLEIDS = 2;
	    public const int SPDRP_CONFIGFLAGS = 0xa;
	    public const int SPDRP_DEVICE_POWER_DATA = 0x1e;
	    public const int SPDRP_DEVICEDESC = 0;
	    public const int SPDRP_DEVTYPE = 0x19;
	    public const int SPDRP_DRIVER = 9;
	    public const int SPDRP_ENUMERATOR_NAME = 0x16;
	    public const int SPDRP_EXCLUSIVE = 0x1a;
	    public const int SPDRP_FRIENDLYNAME = 0xc;
	    public const int SPDRP_HARDWAREID = 1;
	    public const int SPDRP_LEGACYBUSTYPE = 0x14;
	    public const int SPDRP_LOCATION_INFORMATION = 0xd;
	    public const int SPDRP_LOWERFILTERS = 0x12;
	    public const int SPDRP_MFG = 0xb;
	    public const int SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0xe;
	    public const int SPDRP_REMOVAL_POLICY = 0x1f;
	    public const int SPDRP_REMOVAL_POLICY_HW_DEFAULT = 0x20;
	    public const int SPDRP_REMOVAL_POLICY_OVERRIDE = 0x21;
	    public const int SPDRP_SECURITY = 0x17;
	    public const int SPDRP_SECURITY_SDS = 0x18;
	    public const int SPDRP_SERVICE = 4;
	    public const int SPDRP_UI_NUMBER = 0x10;
	    public const int SPDRP_UI_NUMBER_DESC_FORMAT = 0x1d;

        public const int SPDRP_UPPERFILTERS = 0x11;

	    [StructLayout(LayoutKind.Sequential)]
	    public class DEV_BROADCAST_DEVICEINTERFACE
	    {
		    public int dbcc_size;
		    public int dbcc_devicetype;
		    public int dbcc_reserved;
		    public Guid dbcc_classguid;
		    public short dbcc_name;
	    }

	    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	    public class DEV_BROADCAST_DEVICEINTERFACE_1
	    {
		    public int dbcc_size;
		    public int dbcc_devicetype;
		    public int dbcc_reserved;
		    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
		    public byte[] dbcc_classguid;
		    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
		    public char[] dbcc_name;
	    }

	    [StructLayout(LayoutKind.Sequential)]
	    public class DEV_BROADCAST_HANDLE
	    {
		    public int dbch_size;
		    public int dbch_devicetype;
		    public int dbch_reserved;
		    public int dbch_handle;
		    public int dbch_hdevnotify;
	    }

	    [StructLayout(LayoutKind.Sequential)]
	    public class DEV_BROADCAST_HDR
	    {
		    public int dbch_size;
		    public int dbch_devicetype;
		    public int dbch_reserved;
	    }

	    [StructLayout(LayoutKind.Sequential)]
	    public struct SP_DEVICE_INTERFACE_DATA
	    {
		    public int cbSize;
		    public System.Guid InterfaceClassGuid;
		    public int Flags;
		    public IntPtr Reserved;
	    }

	    [StructLayout(LayoutKind.Sequential)]
	    public struct SP_DEVINFO_DATA
	    {
		    public int cbSize;
		    public Guid ClassGuid;
		    public int DevInst;
		    public IntPtr Reserved;
	    }

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int Size;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVPROPKEY
        {
            public Guid fmtid;
            public ulong pid;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_CLASSINSTALL_HEADER
        {
            public int cbSize;
            public int installFunction;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_PROPCHANGE_PARAMS
        {
            public SP_CLASSINSTALL_HEADER classInstallHeader;
            public int stateChange;
            public int scope;
            public int hwProfile;
        }

        public static DEVPROPKEY DEVPKEY_Device_BusReportedDeviceDesc = 
            new DEVPROPKEY { fmtid = new Guid(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2), pid = 4 };

	    [DllImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceRegistryProperty")]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, int propertyVal, ref int propertyRegDataType, byte[] propertyBuffer, int propertyBufferSize, ref int requiredSize);
	
        [DllImport("setupapi.dll", EntryPoint = "SetupDiGetDevicePropertyW", SetLastError = true)]
        public static extern bool SetupDiGetDeviceProperty(IntPtr deviceInfo, ref SP_DEVINFO_DATA deviceInfoData, ref DEVPROPKEY propkey, ref ulong propertyDataType, byte[] propertyBuffer, int propertyBufferSize, ref int requiredSize, uint flags);

	    [DllImport("setupapi.dll")]
	    static public extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet, int memberIndex, ref SP_DEVINFO_DATA deviceInfoData);

	    [DllImport("user32.dll", CharSet = CharSet.Auto)]
	    static public extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr notificationFilter, Int32 flags);

        [DllImport("setupapi.dll")]
        public static extern int SetupDiCreateDeviceInfoList(ref Guid classGuid, int hwndParent);

	    [DllImport("setupapi.dll")]
	    static public extern int SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

	    [DllImport("setupapi.dll")]
        static public extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, ref Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

	    [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static public extern IntPtr SetupDiGetClassDevs(ref System.Guid classGuid, string enumerator, int hwndParent, int flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, EntryPoint = "SetupDiGetDeviceInterfaceDetail")]
        static public extern bool SetupDiGetDeviceInterfaceDetailBuffer(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, IntPtr deviceInfoData);

	    [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
	    static public extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, IntPtr deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static public extern bool SetupDiSetClassInstallParams(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, ref SP_PROPCHANGE_PARAMS classInstallParams, int classInstallParamsSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static public extern bool SetupDiCallClassInstaller(int installFunction, IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static public extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, char[] deviceInstanceId, Int32 deviceInstanceIdSize, ref int requiredSize);

        [DllImport("user32.dll")]
	    static public extern bool UnregisterDeviceNotification(IntPtr handle);

	    public const short HIDP_INPUT = 0;
	    public const short HIDP_OUTPUT = 1;

	    public const short HIDP_FEATURE = 2;
	    [StructLayout(LayoutKind.Sequential)]
	    public struct HIDD_ATTRIBUTES
	    {
		    public int Size;
		    public ushort VendorID;
		    public ushort ProductID;
		    public short VersionNumber;
	    }

	    [StructLayout(LayoutKind.Sequential)]
	    public struct HIDP_CAPS
	    {
		    public short Usage;
		    public short UsagePage;
		    public short InputReportByteLength;
		    public short OutputReportByteLength;
		    public short FeatureReportByteLength;
		    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
		    public short[] Reserved;
		    public short NumberLinkCollectionNodes;
		    public short NumberInputButtonCaps;
		    public short NumberInputValueCaps;
		    public short NumberInputDataIndices;
		    public short NumberOutputButtonCaps;
		    public short NumberOutputValueCaps;
		    public short NumberOutputDataIndices;
		    public short NumberFeatureButtonCaps;
		    public short NumberFeatureValueCaps;
		    public short NumberFeatureDataIndices;
	    }

	    [StructLayout(LayoutKind.Sequential)]
	    public struct HIDP_VALUE_CAPS
	    {
		    public short UsagePage;
		    public byte ReportID;
		    public int IsAlias;
		    public short BitField;
		    public short LinkCollection;
		    public short LinkUsage;
		    public short LinkUsagePage;
		    public int IsRange;
		    public int IsStringRange;
		    public int IsDesignatorRange;
		    public int IsAbsolute;
		    public int HasNull;
		    public byte Reserved;
		    public short BitSize;
		    public short ReportCount;
		    public short Reserved2;
		    public short Reserved3;
		    public short Reserved4;
		    public short Reserved5;
		    public short Reserved6;
		    public int LogicalMin;
		    public int LogicalMax;
		    public int PhysicalMin;
		    public int PhysicalMax;
		    public short UsageMin;
		    public short UsageMax;
		    public short StringMin;
		    public short StringMax;
		    public short DesignatorMin;
		    public short DesignatorMax;
		    public short DataIndexMin;
		    public short DataIndexMax;
	    }

	    [DllImport("hid.dll")]
	    static public extern bool HidD_FlushQueue(IntPtr hidDeviceObject);

        [DllImport("hid.dll")]
        static public extern bool HidD_FlushQueue(SafeFileHandle hidDeviceObject);

	    [DllImport("hid.dll")]
	    static public extern bool HidD_GetAttributes(IntPtr hidDeviceObject, ref HIDD_ATTRIBUTES attributes);

	    [DllImport("hid.dll")]
	    static public extern bool HidD_GetFeature(IntPtr hidDeviceObject, byte[] lpReportBuffer, int reportBufferLength);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern Boolean HidD_GetInputReport(SafeFileHandle HidDeviceObject, Byte[] lpReportBuffer, Int32 ReportBufferLength);        

	    [DllImport("hid.dll")]
	    static public extern void HidD_GetHidGuid(ref Guid hidGuid);

	    [DllImport("hid.dll")]
	    static public extern bool HidD_GetNumInputBuffers(IntPtr hidDeviceObject, ref int numberBuffers);

	    [DllImport("hid.dll")]
	    static public extern bool HidD_GetPreparsedData(IntPtr hidDeviceObject, ref IntPtr preparsedData);

	    [DllImport("hid.dll")]
	    static public extern bool HidD_FreePreparsedData(IntPtr preparsedData);

	    [DllImport("hid.dll")]
        static public extern bool HidD_SetFeature(IntPtr hidDeviceObject, byte[] lpReportBuffer, int reportBufferLength);

        [DllImport("hid.dll")]
        static public extern bool HidD_SetFeature(SafeFileHandle hidDeviceObject, byte[] lpReportBuffer, int reportBufferLength);

	    [DllImport("hid.dll")]
	    static public extern bool HidD_SetNumInputBuffers(IntPtr hidDeviceObject, int numberBuffers);
        
        [DllImport("hid.dll")]
        static public extern bool HidD_SetOutputReport(IntPtr hidDeviceObject, byte[] lpReportBuffer, int reportBufferLength);

        [DllImport("hid.dll", SetLastError = true)]
        static public extern bool HidD_SetOutputReport(SafeFileHandle hidDeviceObject, byte[] lpReportBuffer, int reportBufferLength);

	    [DllImport("hid.dll")]
	    static public extern int HidP_GetCaps(IntPtr preparsedData, ref HIDP_CAPS capabilities);

	    [DllImport("hid.dll")]
	    static public extern int HidP_GetValueCaps(short reportType, ref byte valueCaps, ref short valueCapsLength, IntPtr preparsedData);

        [DllImport("hid.dll")]
        static public extern bool HidD_GetSerialNumberString(IntPtr HidDeviceObject, byte[] Buffer, uint BufferLength);
    }
}
