using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Race Property/Gender")]
    [Objforming.Referable]
    public class KeywordsRogueGenderAsset : RogueGenderAsset
    {
        [SerializeField] private KeywordAsset[] _keywords = null;

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
