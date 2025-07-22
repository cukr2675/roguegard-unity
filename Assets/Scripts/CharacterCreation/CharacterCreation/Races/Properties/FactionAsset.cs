using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Race Property/Faction")]
    public class FactionAsset : ScriptableObject
    {
        [SerializeField] private SerializableKeywordAsset _faction;
        public ISerializableKeyword Faction => _faction;

        [SerializeField] private SerializableKeywordAsset[] _targetFactions;
        public Spanning<ISerializableKeyword> TargetFactions => _targetFactions;
    }
}
