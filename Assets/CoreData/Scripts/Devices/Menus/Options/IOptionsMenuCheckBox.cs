using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public interface IOptionsMenuCheckBox
    {
        string GetName(MMgr manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        bool GetValue(MMgr manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void SetValue(MMgr manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, bool value);
    }
}
