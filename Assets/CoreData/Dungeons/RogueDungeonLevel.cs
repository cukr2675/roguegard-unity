using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public abstract class RogueDungeonLevel : ScriptableObject
    {
        [SerializeField] private int _endLv = 0;
        public int EndLv => _endLv;

        public abstract Spanning<IWeightedRogueObjGeneratorList> EnemyTable { get; }
        public abstract Spanning<IWeightedRogueObjGeneratorList> ItemTable { get; }
        public abstract Spanning<IWeightedRogueObjGeneratorList> OtherTable { get; }

        public abstract void GenerateFloor(RogueObj player, RogueObj floor, IRogueRandom random);
    }
}
