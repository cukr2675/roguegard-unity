using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public interface IOptionsMenuSlider
    {
        string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        float GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value);
    }
}
