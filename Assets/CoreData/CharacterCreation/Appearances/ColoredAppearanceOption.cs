using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            BoneNodeBuilder mainNode, AffectableBoneSpriteTable boneSpriteTable, IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
            if (IsBone)
            {
                Recursion(mainNode);
            }
            else
            {
                var sprite = GetSprite(appearance, characterCreationData);
                boneSpriteTable.AddEquipmentSprite(BoneName, sprite, appearance.Color);
                return;
            }

            void Recursion(BoneNodeBuilder node)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    var child = node.Children[i];
                    if (child.Bone.Name is IKeyword name && name == BoneName)
                    {
                        var sprite = GetSprite(appearance, characterCreationData);
                        child.Bone = new Bone(child.Bone, sprite, appearance.Color);
                    }
                    Recursion(child);
                }
            }
        }

        private class Bone : IBone
        {
            private readonly IBone baseBone;
            private readonly BoneSprite _sprite;
            private readonly Color _color;

            public IKeyword Name => baseBone.Name;
            public BoneSprite Sprite => _sprite;
            public Color Color => _color;
            public bool OverridesBaseColor => baseBone.OverridesBaseColor;
            public bool FlipX => baseBone.FlipX;
            public bool FlipY => baseBone.FlipY;
            public Vector3 LocalPosition => baseBone.LocalPosition;
            public Quaternion LocalRotation => baseBone.LocalRotation;
            public Vector3 ScaleOfLocalByLocal => baseBone.ScaleOfLocalByLocal;
            public float NormalOrderInParent => baseBone.NormalOrderInParent;
            public float BackOrderInParent => baseBone.BackOrderInParent;

            public Bone(IBone baseBone, BoneSprite sprite, Color color)
            {
                this.baseBone = baseBone;
                _sprite = sprite;
                _color = color;
            }
        }
    }
}
