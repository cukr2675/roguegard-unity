using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "RoguegardData/Material")]
    public class KeywordsRogueMaterial : RogueMaterial
    {
        [SerializeField] private KeywordData[] _keywords = null;

        public override void AffectValue(EffectableValue value, RogueObj self)
        {
            foreach (var keyword in _keywords)
            {
                value.SubValues[keyword] = 1f;
            }
        }
    }
}
