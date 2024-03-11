using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Roguegard.Device
{
    public interface IModelsMenuOptionText
    {
        TMP_InputField.ContentType ContentType { get; }

        string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value);
    }
}
