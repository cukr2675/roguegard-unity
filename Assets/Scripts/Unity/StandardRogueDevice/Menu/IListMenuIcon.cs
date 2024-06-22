using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    internal interface IListMenuIcon
    {
        public void GetIcon(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, out Sprite sprite, out Color color);
    }
}
