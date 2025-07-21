using OchalikeSprites;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public abstract class ColoredAppearanceOption : AppearanceOption
    {
        [Space]
        [SerializeField] private BoneKeywordData _boneName = null;
        public BoneKeywordData BoneNameSource { get => _boneName; set => _boneName = value; }
        public override BoneKeyword BoneName => _boneName;

        [SerializeField] private bool _isBone = false;
        public bool IsBone { get => _isBone; set => _isBone = value; }

        protected abstract BoneSprite GetSprite(IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData);

        public sealed override void Affect(
            OchalikeBone mainBone, AppearanceMorph morph, IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
            if (IsBone)
            {
                Recursion(mainBone);
            }
            else
            {
                var sprite = GetSprite(appearance, characterCreationData);
                morph.BaseEffectOchalikeMorph.AddEquipmentSprite(BoneName, sprite, appearance.Color);
                return;
            }

            void Recursion(OchalikeBone bone)
            {
                for (int i = 0; i < bone.Children.Count; i++)
                {
                    var child = bone.Children[i];
                    if (child.Name == BoneName)
                    {
                        var sprite = GetSprite(appearance, characterCreationData);
                        child.BareSprite = sprite;
                        child.BareColor = appearance.Color;
                    }
                    Recursion(child);
                }
            }
        }
    }
}
