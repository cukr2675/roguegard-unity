using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public interface IOptionsMenuColor
    {
        string GetName(MMgr manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        Color GetValue(MMgr manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void SetValue(MMgr manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, Color value);
    }
}
