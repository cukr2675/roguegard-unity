using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OchalikeSprites;

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
            OchalikeBone mainBone, AppearanceBoneSpriteTable boneSpriteTable, IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
            if (IsBone)
            {
                Recursion(mainBone);
            }
            else
            {
                var sprite = GetSprite(appearance, characterCreationData);
                boneSpriteTable.BaseTable.AddEquipmentSprite(BoneName, sprite, appearance.Color);
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
                        child.Sprite = sprite;
                        child.Color = appearance.Color;
                    }
                    Recursion(child);
                }
            }
        }
    }
}
