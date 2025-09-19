using OchalikeSprites;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Appearance/Morph")]
    [Objforming.Referable]
    public class MorphAppearanceOptionAsset : AppearanceOptionAsset
    {
        [Space]
        [SerializeField] private BoneKeywordAsset _boneName = null;
        public BoneKeywordAsset BoneNameSource { get => _boneName; set => _boneName = value; }
        public override BoneKeyword BoneName => _boneName;

        [SerializeField] private bool _isBone = false;
        public bool IsBone { get => _isBone; set => _isBone = value; }

        [SerializeField] private ColorRangedBoneSprite _sprite = null;
        public ColorRangedBoneSprite Sprite { get => _sprite; set => _sprite = value; }

        [SerializeField] private OchalikeMorphAsset _morph = null;
        public OchalikeMorphAsset Morph { get => _morph; set => _morph = value; }

        public sealed override void Affect(
            OchalikeBone mainBone, AppearanceMorph morph, IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
            if (IsBone)
            {
                var tempMorph = new OchalikeMorph();
                _morph.AddTo(tempMorph);
                Recursion(mainBone, tempMorph);
            }
            else
            {
                _morph.ColoredAddTo(morph.BaseEffectOchalikeMorph, appearance.Color);
                return;
            }

            void Recursion(OchalikeBone bone, OchalikeMorph tempMorph)
            {
                for (int i = 0; i < bone.Children.Count; i++)
                {
                    var child = bone.Children[i];
                    var item = tempMorph.GetSprite(child.Name);
                    if (item.MorphBareSprite != null)
                    {
                        child.BareSprite = item.MorphBareSprite;
                        child.BareColor = appearance.Color;
                    }
                    Recursion(child, tempMorph);
                }
            }
        }
    }
}
