using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public interface IOptionsMenuColor
    {
        string GetName(RogueMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        Color GetValue(RogueMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void SetValue(RogueMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, Color value);
    }
}
