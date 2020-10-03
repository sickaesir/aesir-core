using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScarlettLib
{
    public static class Helpers
    {
        public static byte[] GetPacket(string data)
        {
            string str = $"Length={(data.Length + 1).ToString("X6")} {data}" + (char)0x0a;

            byte[] bytes = Encoding.UTF8.GetBytes(str);

            return bytes;
        }
    }
}
