using System;
using System.Collections.Generic;
using System.Text;
using NAudio;
using NAudio.Midi;

namespace ConsoleMidi
{
    public class ConsoleMidi
    {
        MidiOut midi;
        public ConsoleMidi()
        {
            Bootstrap();
        }

        private void Bootstrap()
        {
            string[] devices = new string[MidiOut.NumberOfDevices];
            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                if (MidiOut.DeviceInfo(i).ProductName == "DDJ-SB3")
                {
                    midi = new MidiOut(i);
                }
            }
        }

        public void SetMidiNote(int note, int channel, int on)
        {
            var noteEvent = new ControlChangeEvent()
            midi.Send(noteEvent.GetAsShortMessage());

        }
    }
}
