using System;
using System.Threading;

namespace ConsoleMidiTester
{
    class Program
    {
        static ConsoleMidi.ConsoleMidi midi;
        static void Main(string[] args)
        {
            midi = new ConsoleMidi.ConsoleMidi();
            midi.SetMidiNote(102, 1, 1);

            while (true)
            {
                for (int i = 5; i < 16; i++)
                {
                    for (int j = 100; j < 127; j++)
                    {
                        midi.SetMidiNote(j, i, 100000);
                        Console.WriteLine($"Executing note {j} channel {i}");
                        Thread.Sleep(1000);
                    }

                   Thread.Sleep(10000);
                }
            }
        }
    }
}
