using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Roguegard.Device
{
    public interface IOptionsMenuText
    {
        TMP_InputField.ContentType ContentType { get; }

        string GetName(RogueMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        string GetValue(RogueMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void SetValue(RogueMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value);
    }
}
