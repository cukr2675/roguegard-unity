using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace Roguegard
{
    public interface IModelsMenuOptionText
    {
        string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value);
    }
}
