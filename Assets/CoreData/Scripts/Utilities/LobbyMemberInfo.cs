using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    [Objforming.Formable]
    public class LobbyMemberInfo
    {
        public CharacterCreationDataBuilder CharacterCreationData { get; set; }

        public ISavePointInfo SavePoint { get; set; }

        public RogueObj Seat { get; set; }

        public RogueObjRegister ItemRegister { get; } = new RogueObjRegister();
    }
}
