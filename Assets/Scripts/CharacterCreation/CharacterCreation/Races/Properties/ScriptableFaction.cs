using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Race Property/Faction")]
    public class ScriptableFaction : ScriptableObject
    {
        [SerializeField] private SerializableKeywordData _faction;
        public ISerializableKeyword Faction => _faction;

        [SerializeField] private SerializableKeywordData[] _targetFactions;
        public Spanning<ISerializableKeyword> TargetFactions => _targetFactions;
    }
}
