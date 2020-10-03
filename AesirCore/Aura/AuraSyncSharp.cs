using AesirCore.Aura;
using AesirCore.Controllers;
using AesirLights.Structs;
using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AesirCore
{
    public static class AuraSyncSharp
    {
        public static List<AuraDeviceController> Controllers { private set; get; }
        private static object controllersLock;

        public static void Init()
        {
            AuraIO.Init();
            controllersLock = new object();
            Controllers = AuraIO.CreateControllers().ToList();

            AuraIO.SetMotherBoardMode(1);
            AuraIO.SetGFXCardMode(1);

            Thread.Sleep(50);

            new Thread(PollThread).Start();
        }

        public static void SetLedRgb(LedType ledType, uint idx, Rgb rgb)
        {
            int controllerIdx = Controllers.FindIndex(k => k.ledType == ledType && k.ledIdx == idx);

            if (controllerIdx < 0) return;

            lock (controllersLock)
                Controllers[controllerIdx].SetRgb(rgb);
        }

        private static void PollThread()
        {
            while(true)
            {
                if (!Controllers.Exists(r => r.invalidated))
                {
                    Thread.Sleep(50);
                    continue;
                }

                byte[] moboColors = new byte[3 * Controllers.Where(k => k.ledType == LedType.Motherboard).Count()];
                byte[] vgaColors = new byte[3 * Controllers.Where(k => k.ledType == LedType.Vga).Count()];

                lock(controllersLock)
                {
                    foreach (AuraDeviceController controller in Controllers)
                    {
                        switch (controller.ledType)
                        {
                            case LedType.Motherboard:
                                moboColors[controller.ledIdx * 3] = controller.CurrentRGB.r;
                                moboColors[controller.ledIdx * 3 + 2] = controller.CurrentRGB.g;
                                moboColors[controller.ledIdx * 3 + 1] = controller.CurrentRGB.b;
                                break;

                            case LedType.Vga:
                                vgaColors[controller.ledIdx * 3] = controller.CurrentRGB.r;
                                vgaColors[controller.ledIdx * 3 + 1] = controller.CurrentRGB.g;
                                vgaColors[controller.ledIdx * 3 + 2] = controller.CurrentRGB.b;
                                break;
                        }
                    }
                }

                AuraIO.SetMotherBoardColors(moboColors);
                AuraIO.SetGFXCardColors(vgaColors);

                for (int i = 0; i < Controllers.Count; i++)
                    Controllers[i].OnLedSet();

                Thread.Sleep(50);
            }
        }
    }
}
