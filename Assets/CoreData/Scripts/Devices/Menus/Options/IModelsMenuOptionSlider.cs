using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace Roguegard
{
    public interface IModelsMenuOptionSlider
    {
        string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        float GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value);
    }
}
