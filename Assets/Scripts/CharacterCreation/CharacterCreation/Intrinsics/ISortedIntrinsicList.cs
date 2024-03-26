using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface ISortedIntrinsicList
    {
        void Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base);

        void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph);

        void Reopen(RogueObj self, MainInfoSetType infoSetType, int deltaLv);
    }
}
