using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class ObjCommandTableAsset : ScriptableObject
    {
        public abstract Spanning<IKeyword> Categories { get; }

        public abstract IObjCommand PickUpCommand { get; }

        public abstract void GetCommands(RogueObj self, RogueObj tool, IList<IObjCommand> commands);
    }
}
