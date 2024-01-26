using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    internal interface IModelsMenuIcon
    {
        public void GetIcon(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, out Sprite sprite, out Color color);
    }
}
