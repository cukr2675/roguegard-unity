using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public interface IModelsMenuOptionSlider
    {
        string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        float GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value);
    }
}
