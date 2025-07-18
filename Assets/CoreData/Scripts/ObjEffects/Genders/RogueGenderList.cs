using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "RoguegardData/GenderList")]
    public class RogueGenderList : ScriptableObject
    {
        [SerializeField] private RogueGender[] _genders = null;

        public RogueGender this[int index] => _genders[index];

        public int Count => _genders.Length;

        public Spanning<IRogueGender> Span => _genders;
    }
}
