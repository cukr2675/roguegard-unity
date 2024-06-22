using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public abstract class BaseListMenuSelectOption : IListMenuSelectOption
    {
        public abstract string Name { get; }

        string IListMenuSelectOption.GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            return Name;
        }

        public abstract void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);
    }
}
