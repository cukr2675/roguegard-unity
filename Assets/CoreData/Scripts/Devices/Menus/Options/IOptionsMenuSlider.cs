using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public interface IOptionsMenuSlider
    {
        string GetName(RogueMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        float GetValue(RogueMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void SetValue(RogueMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value);
    }
}
