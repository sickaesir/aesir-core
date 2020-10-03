using CommonLib;
using DS4Library;
using ScarlettLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DS4LibTest
{
    class Program
    {
        static DS4Device device;
        static ScarlettService service;
        static ConsoleMidi midi;
        static void Main(string[] args)
        {
            service = new ScarlettService();
            Thread.Sleep(-1);
        }

    }
}
