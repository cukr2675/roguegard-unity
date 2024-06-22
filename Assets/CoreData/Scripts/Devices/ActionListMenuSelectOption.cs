using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class ActionListMenuSelectOption : BaseListMenuSelectOption
    {
        private readonly ListMenuAction action;

        public override string Name { get; }

        public ActionListMenuSelectOption(string name, ListMenuAction action)
        {
            Name = name;
            this.action = action;
        }

        public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            action.Invoke(manager, self, user, arg);
        }
    }
}
