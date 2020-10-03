using HidLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS4Library
{
    public class DS4ConnectionType
    {
        public enum Type
        {
            Bluetooth,
            USB
        }

        public static DS4ConnectionType.Type GetConnectionType(HidDevice hidDevice)
        {
            return hidDevice.Capabilities.InputReportByteLength == DS4Constants.DS4_USB_INPUT_REPORT_LENGTH ? Type.USB : Type.Bluetooth;
        }
    }
}
