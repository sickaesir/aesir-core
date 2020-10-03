using DS4Library.State;
using DS4Library.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS4Library.State
{
    public class DS4State
    {
        public DateTime ReportTimeStamp { get; private set; }

        private Dictionary<ButtonType, DS4Button> buttonsStates;
        private Dictionary<StickType, DS4Stick> sticksStates;
        private Dictionary<TriggerType, DS4Trigger> triggersStates;

        public DS4TouchPad TouchPad { get; private set; }
        public DS4Accelerometer Accelerometer { get; private set; }
        public DS4Gyroscope Gyroscope { get; private set; }

        public bool Charging { private set; get; }
        public byte BatteryLevel { private set; get; }

        // events
        public event EventHandler OnStartCharging;
        public event EventHandler OnStopCharging;
        public event EventHandler OnBatteryLevelChange;

        public DS4State()
        {
            buttonsStates = new Dictionary<ButtonType, DS4Button>();
            sticksStates = new Dictionary<StickType, DS4Stick>();
            triggersStates = new Dictionary<TriggerType, DS4Trigger>();

            for (ButtonType i = ButtonType._First + 1; i < ButtonType._Last; i++)
            {
                buttonsStates.Add(i, new DS4Button(i));
            }

            for (StickType i = StickType._First + 1; i < StickType._Last; i++)
            {
                sticksStates.Add(i, new DS4Stick(i));
            }

            for (TriggerType i = TriggerType._First + 1; i < TriggerType._Last; i++)
            {
                triggersStates.Add(i, new DS4Trigger(i));
            }

            TouchPad = new DS4TouchPad();
            Accelerometer = new DS4Accelerometer();
            Gyroscope = new DS4Gyroscope();

            OnStartCharging = null;
            OnStopCharging = null;
            OnBatteryLevelChange = null;

        }

        public DS4Button GetButton(ButtonType button)
        {
            if (!buttonsStates.ContainsKey(button)) return null;

            return buttonsStates[button];
        }

        public IEnumerable<DS4Button> GetButtons()
        {
            return buttonsStates.Values;
        }

        public IEnumerable<DS4Stick> GetSticks()
        {
            return sticksStates.Values;
        }

        public IEnumerable<DS4Trigger> GetTriggers()
        {
            return triggersStates.Values;
        }

        private void SetButtonState(ButtonType button, bool state)
        {
            if (!buttonsStates.ContainsKey(button)) return;

            buttonsStates[button].SetButtonState(state);
        }

        public DS4Stick GetStick(StickType stick)
        {
            if (!sticksStates.ContainsKey(stick)) return null;

            return sticksStates[stick];
        }

        public DS4Trigger GetTrigger(TriggerType trigger)
        {
            if (!triggersStates.ContainsKey(trigger)) return null;

            return triggersStates[trigger];
        }

        internal void ParseFromReport(byte[] reportData)
        {
            ReportTimeStamp = System.DateTime.UtcNow;

            foreach (var stick in sticksStates)
                stick.Value.ParseFromReport(reportData);

            foreach (var trigger in triggersStates)
                trigger.Value.ParseFromReport(reportData);

            TouchPad.ParseFromReport(reportData);
            Accelerometer.ParseFromReport(reportData);
            Gyroscope.ParseFromReport(reportData);

            SetButtonState(ButtonType.Triangle, ((byte)reportData[5] & (1 << 7)) != 0);
            SetButtonState(ButtonType.Circle, ((byte)reportData[5] & (1 << 6)) != 0);
            SetButtonState(ButtonType.Cross, ((byte)reportData[5] & (1 << 5)) != 0);
            SetButtonState(ButtonType.Square, ((byte)reportData[5] & (1 << 4)) != 0);


            byte dpad_state = (byte)(((((byte)reportData[5] & (1 << 0)) != 0 ? 1 : 0) << 0) | ((((byte)reportData[5] & (1 << 1)) != 0 ? 1 : 0) << 1) | ((((byte)reportData[5] & (1 << 2)) != 0 ? 1 : 0) << 2) | ((((byte)reportData[5] & (1 << 3)) != 0 ? 1 : 0) << 3));

            switch (dpad_state)
            {
                case 0:
                    SetButtonState(ButtonType.Up, true);
                    SetButtonState(ButtonType.Down, false);
                    SetButtonState(ButtonType.Left, false);
                    SetButtonState(ButtonType.Right, false);
                    break;

                case 1:
                    SetButtonState(ButtonType.Up, true);
                    SetButtonState(ButtonType.Down, false);
                    SetButtonState(ButtonType.Left, false);
                    SetButtonState(ButtonType.Right, true);
                    break;

                case 2:
                    SetButtonState(ButtonType.Up, false);
                    SetButtonState(ButtonType.Down, false);
                    SetButtonState(ButtonType.Left, false);
                    SetButtonState(ButtonType.Right, true);
                    break;

                case 3:
                    SetButtonState(ButtonType.Up, false);
                    SetButtonState(ButtonType.Down, true);
                    SetButtonState(ButtonType.Left, false);
                    SetButtonState(ButtonType.Right, true);
                    break;

                case 4:
                    SetButtonState(ButtonType.Up, false);
                    SetButtonState(ButtonType.Down, true);
                    SetButtonState(ButtonType.Left, false);
                    SetButtonState(ButtonType.Right, false);
                    break;

                case 5:
                    SetButtonState(ButtonType.Up, false);
                    SetButtonState(ButtonType.Down, true);
                    SetButtonState(ButtonType.Left, true);
                    SetButtonState(ButtonType.Right, false);
                    break;

                case 6:
                    SetButtonState(ButtonType.Up, false);
                    SetButtonState(ButtonType.Down, false);
                    SetButtonState(ButtonType.Left, true);
                    SetButtonState(ButtonType.Right, false);
                    break;

                case 7:
                    SetButtonState(ButtonType.Up, true);
                    SetButtonState(ButtonType.Down, false);
                    SetButtonState(ButtonType.Left, true);
                    SetButtonState(ButtonType.Right, false);
                    break;

                case 8:
                    SetButtonState(ButtonType.Up, false);
                    SetButtonState(ButtonType.Down, false);
                    SetButtonState(ButtonType.Left, false);
                    SetButtonState(ButtonType.Right, false);
                    break;

            }

            SetButtonState(ButtonType.R3, ((byte)reportData[6] & (1 << 7)) != 0);
            SetButtonState(ButtonType.L3, ((byte)reportData[6] & (1 << 6)) != 0);
            SetButtonState(ButtonType.Options, ((byte)reportData[6] & (1 << 5)) != 0);
            SetButtonState(ButtonType.Share, ((byte)reportData[6] & (1 << 4)) != 0);
            SetButtonState(ButtonType.L1, ((byte)reportData[6] & (1 << 0)) != 0);
            SetButtonState(ButtonType.R1, ((byte)reportData[6] & (1 << 1)) != 0);
            SetButtonState(ButtonType.PS, ((byte)reportData[7] & (1 << 0)) != 0);
            SetButtonState(ButtonType.Touchpad, (reportData[7] & (1 << 2 - 1)) != 0);

            bool isCharging = (reportData[30] & 0x10) != 0;

            if(Charging != isCharging)
            {
                Charging = isCharging;
                if (isCharging)
                    OnStartCharging?.Invoke(this, new EventArgs());
                else
                    OnStopCharging?.Invoke(this, new EventArgs());
            }

            byte batteryLevel = (byte)((reportData[30] & 0x0F) * 10);

            if(BatteryLevel != batteryLevel)
            {
                BatteryLevel = batteryLevel;
                OnBatteryLevelChange?.Invoke(this, new EventArgs());
            }
        }


    }
}
