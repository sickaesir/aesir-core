using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS4Library
{
    class DS4Constants
    {
        internal static readonly int DS4_BT_OUTPUT_REPORT_LENGTH = 78;
        internal static readonly int DS4_BT_INPUT_REPORT_LENGTH = 547;
        internal static readonly int DS4_USB_INPUT_REPORT_LENGTH = 64;
        internal static readonly int DS4_IO_TIMEOUT_MS = 3000;
        internal static readonly int DS4_IO_HANDSHAKE_TIMEOUT_MS = DS4_IO_TIMEOUT_MS * 10;
        internal static readonly int DS4_TOUCHPAD_TOUCH_CAPACITY = 2;
        internal static readonly int DS4_HID_VID = 0x054C;
        internal static readonly int[] DS4_HID_PIDS = { 0xba0, 0x5c4, 0x09cc };
    }
}
