using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "RoguegardData/Keyword/Equip")]
    [ObjectFormer.Referable]
    public class EquipKeywordData : SerializableKeywordData
    {
        [Header("Equip")]
        [SerializeField] private float _order = 0f;
        public float Order => _order;
    }
}
