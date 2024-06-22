using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public interface IOptionsMenuCheckBox
    {
        string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        bool GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, bool value);
    }
}
