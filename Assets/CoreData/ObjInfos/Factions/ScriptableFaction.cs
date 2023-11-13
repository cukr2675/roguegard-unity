using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Race/Faction")]
    public class ScriptableFaction : ScriptableObject
    {
        [SerializeField] private SerializableKeywordData _faction;
        public ISerializableKeyword Faction => _faction;

        [SerializeField] private SerializableKeywordData[] _targetFactions;
        public Spanning<ISerializableKeyword> TargetFactions => _targetFactions;
    }
}
