using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public interface IModelsMenuOptionCheckBox
    {
        string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        bool GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, bool value);
    }
}
