using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.RequireRelationalComponent]
    public interface IMember : IReadOnlyMember
    {
        void SetRandom(ICharacterCreationDatabase database, IRogueRandom random);
    }
}
