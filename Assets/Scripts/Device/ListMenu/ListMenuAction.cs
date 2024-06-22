using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public delegate void ListMenuAction(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);
}
