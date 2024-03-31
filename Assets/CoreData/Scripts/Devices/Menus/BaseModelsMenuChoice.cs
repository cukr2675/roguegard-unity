using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public abstract class BaseModelsMenuChoice : IModelsMenuChoice
    {
        public abstract string Name { get; }

        string IModelsMenuChoice.GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            return Name;
        }

        public abstract void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);
    }
}
