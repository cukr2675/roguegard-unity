using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class ActionListMenuSelectOption : BaseListMenuSelectOption
    {
        public ActionListMenuSelectOption(string name, HandleClickAction action)
        {
            Name = name;
            HandleClick = action;
        }
    }
}
