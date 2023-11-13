using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "RoguegardData/GenderList")]
    public class RogueGenderList : ScriptableObject//, IReadOnlyList<RogueGender>
    {
        [SerializeField] private RogueGender[] _genders = null;

        public RogueGender this[int index] => _genders[index];

        public int Count => _genders.Length;

        private IEnumerator<RogueGender> GetEnumerator() => ((IEnumerable<RogueGender>)_genders).GetEnumerator();
        public static implicit operator Spanning<IRogueGender>(RogueGenderList list) => list._genders;
    }
}
