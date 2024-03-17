using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard.CharacterCreation
{
    public abstract class ColoredAppearanceOption : AppearanceOption
    {
        [Space]
        [SerializeField] private KeywordData _boneName = null;
        public KeywordData BoneNameSource { get => _boneName; set => _boneName = value; }
        public override IKeyword BoneName => _boneName;

        [SerializeField] private bool _isBone = false;
        public bool IsBone { get => _isBone; set => _isBone = value; }

        protected abstract BoneSprite GetSprite(IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData);

        public sealed override void Affect(
            NodeBone mainNode, AppearanceBoneSpriteTable boneSpriteTable, IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
            if (IsBone)
            {
                Recursion(mainNode);
            }
            else
            {
                var sprite = GetSprite(appearance, characterCreationData);
                boneSpriteTable.BaseTable.AddEquipmentSprite(new BoneKeyword(BoneName.Name), sprite, appearance.Color);
                return;
            }

            void Recursion(NodeBone node)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    var child = node.Children[i];
                    if (child.Name == new BoneKeyword(BoneName.Name))
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
