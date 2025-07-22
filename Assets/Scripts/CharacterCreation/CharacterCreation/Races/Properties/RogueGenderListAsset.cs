using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Race Property/Gender List")]
    public class RogueGenderListAsset : ScriptableObject
    {
        [SerializeField] private RogueGenderAsset[] _genders = null;

        public RogueGenderAsset this[int index] => _genders[index];

        public int Count => _genders.Length;

        public Spanning<IRogueGender> Span => _genders;
    }
}
