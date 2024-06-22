using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Roguegard.Device
{
    public interface IOptionsMenuText
    {
        TMP_InputField.ContentType ContentType { get; }

        string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value);
    }
}
