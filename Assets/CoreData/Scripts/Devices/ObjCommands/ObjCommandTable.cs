using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class ObjCommandTable : ScriptableObject
    {
        public abstract Spanning<IKeyword> Categories { get; }

        public abstract IObjCommand PickUpCommand { get; }

        public abstract void GetCommands(RogueObj self, RogueObj tool, IList<IObjCommand> commands);
    }
}
