using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;

namespace Roguegard.Device
{
    public class ActionListMenuSelectOption : BaseListMenuSelectOption
    {
        public ActionListMenuSelectOption(string name, HandleClickElement<RogueMenuManager, ReadOnlyMenuArg> action)
        {
            Name = name;
            HandleClick = action;
        }
    }
}
