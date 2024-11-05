using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Roguegard.Device
{
    public interface IOptionsMenuText
    {
        TMP_InputField.ContentType ContentType { get; }

        string GetName(MMgr manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        string GetValue(MMgr manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void SetValue(MMgr manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value);
    }
}
