using AesirDs4.Structs;
using AudioSwitcher.AudioApi.CoreAudio;
using CommonLib;
using DS4Library.State;
using DS4Library.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AesirDs4.Menu
{
    class MusicMenu : MenuBase
    {

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);

        public const int KEYEVENTF_EXTENTEDKEY = 1;
        public const int KEYEVENTF_KEYUP = 0;
        public const int VK_MEDIA_NEXT_TRACK = 0xB0;
        public const int VK_MEDIA_PLAY_PAUSE = 0xB3;
        public const int VK_MEDIA_PREV_TRACK = 0xB1;

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);

        private const int WM_APPCOMMAND = 0x319;

        CoreAudioController audioController;
        public MusicMenu() : base(new Rgb(0, 255, 0))
        {
            audioController = new CoreAudioController();
        }

        public override void OnButtonStateChange(ButtonType button, bool state)
        {
            if (!Active) return;
            switch(button)
            {
                case ButtonType.Right:

                    if(state)
                        keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);

                    break;

                case ButtonType.Left:

                    if(state)
                        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);

                    break;

                case ButtonType.Touchpad:

                    if (state)
                        keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);

                    break;
            }
        }

        public override void OnStateUpdate(DS4State state)
        {
            if (!Active) return;
            DS4Stick mediaStick = state.GetStick(StickType.L);


            if (mediaStick.Y == 1 && audioController.DefaultPlaybackDevice.Volume > 0)
            {
                audioController.DefaultPlaybackDevice.Volume--;
            }
            else if (mediaStick.Y == -1 && audioController.DefaultPlaybackDevice.Volume < 100)
            {
                audioController.DefaultPlaybackDevice.Volume++;
            }
        }

    }
}
