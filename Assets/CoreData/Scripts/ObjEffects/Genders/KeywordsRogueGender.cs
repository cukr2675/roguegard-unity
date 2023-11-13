using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "RoguegardData/Gender")]
    [ObjectFormer.Referable]
    public class KeywordsRogueGender : RogueGender
    {
        [SerializeField] private KeywordData[] _keywords = null;

        public override void AffectValue(AffectableValue value, RogueObj self, MainInfoSetType infoSetType)
        {
            if (infoSetType != MainInfoSetType.Base) return;

            foreach (var keyword in _keywords)
            {
                value.SubValues[keyword] = 1f;
            }
        }
    }
}
