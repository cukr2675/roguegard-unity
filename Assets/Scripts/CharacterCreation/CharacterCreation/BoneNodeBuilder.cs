using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using SkeletalSprite;

namespace Roguegard.CharacterCreation
{
    public class BoneNodeBuilder : IBoneNode
    {
        public IBone Bone { get; set; }

        private readonly BoneNodeBuilder[] _children;
        public Spanning<BoneNodeBuilder> Children => _children;
        IReadOnlyList<IBoneNode> IBoneNode.Children => _children;

        public BoneNodeBuilder(IBone bone, IEnumerable<BoneNodeBuilder> children)
        {
            Bone = bone;
            _children = children.ToArray();
        }

        public static BoneNodeBuilder Create(BoneData boneData, Color color)
        {
            return Recursion(0);

            BoneNodeBuilder Recursion(int index)
            {
                var boneNode = boneData[index];
                var bone = new BuilderBone(boneNode, color);
                var children = new List<BoneNodeBuilder>();
                var startIndex = index + 1;
                for (int i = startIndex; i < boneData.Count; i++)
                {
                    var itemNode = boneData[i];
                    if (itemNode.ParentBoneName != boneNode.BoneName) continue;

                    var child = Recursion(i);
                    children.Add(child);
                }
                return new BoneNodeBuilder(bone, children);
            }
        }

        private class BuilderBone : IBone
        {
            public BoneKeyword Name { get; set; }
            public BoneSprite Sprite { get; set; }
            public Color Color { get; set; }
            public bool OverridesBaseColor { get; set; }
            public bool FlipX { get; set; }
            public bool FlipY { get; set; }
            public Vector3 LocalPosition { get; set; }
            public Quaternion LocalRotation { get; set; }
            public Vector3 ScaleOfLocalByLocal { get; set; }
            public float NormalOrderInParent { get; set; }
            public float BackOrderInParent { get; set; }

            public BuilderBone(BoneData.Node node, Color color)
            {
                Name = node.BoneName;
                Sprite = node.Sprite.GetSprite(color, node.LightDarkThreshold);
                Color = color;
                OverridesBaseColor = node.OverridesBaseColor;
                FlipX = node.FlipX;
                FlipY = node.FlipY;
                LocalPosition = node.PixelLocalPosition / node.PixelsPerUnit;
                LocalRotation = node.LocalRotation;
                ScaleOfLocalByLocal = node.ScaleOfLocalByLocal;
                NormalOrderInParent = node.NormalOrderInParent;
                BackOrderInParent = node.BackOrderInParent;
            }
        }
    }
}
