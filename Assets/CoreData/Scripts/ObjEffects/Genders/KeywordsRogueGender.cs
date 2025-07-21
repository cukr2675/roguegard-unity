using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "Roguegard/Gender")]
    [Objforming.Referable]
    public class KeywordsRogueGender : RogueGender
    {
        [SerializeField] private KeywordData[] _keywords = null;

        public override void AffectValue(EffectableValue value, RogueObj self, MainInfoSetType infoSetType)
        {
            if (infoSetType != MainInfoSetType.Base) return;

            foreach (var keyword in _keywords)
            {
                value.SubValues[keyword] = 1f;
            }
        }
    }
}
