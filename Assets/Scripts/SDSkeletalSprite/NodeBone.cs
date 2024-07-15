using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDSSprite
{
    public class NodeBone : IReadOnlyNodeBone
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

        public BoneList Children { get; } = new BoneList();

        IReadOnlyList<IReadOnlyNodeBone> IReadOnlyNodeBone.Children => Children;
    }
}
