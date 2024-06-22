using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public interface IOptionsMenuColor
    {
        string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        Color GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, Color value);
    }
}
