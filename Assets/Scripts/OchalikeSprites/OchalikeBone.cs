using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    public class OchalikeBone : IReadOnlyOchalikeBone
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

        public OchalikeBoneList Children { get; } = new OchalikeBoneList();

        IReadOnlyList<IReadOnlyOchalikeBone> IReadOnlyOchalikeBone.Children => Children;
    }
}
