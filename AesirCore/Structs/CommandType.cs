using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AesirLights.Structs
{
    public enum CommandType
    {
        GetEffects = 0,
        GetControllers,
        SetControllerEffect,
        SetControllerBrightness
    }
}
