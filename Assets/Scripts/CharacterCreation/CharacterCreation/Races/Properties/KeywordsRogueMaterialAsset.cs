using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Race Property/Material")]
    public class KeywordsRogueMaterialAsset : RogueMaterialAsset
    {
        [SerializeField] private KeywordAsset[] _keywords = null;

        public override void AffectValue(EffectableValue value, RogueObj self)
        {
            foreach (var keyword in _keywords)
            {
                value.SubValues[keyword] = 1f;
            }
        }
    }
}
