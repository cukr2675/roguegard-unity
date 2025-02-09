using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    /// <summary>
    /// <see cref="OchalikeSpriteData"/> を構成するボーン。
    /// これをもとに <see cref="OchalikeMorph"/> と <see cref="SpritePose"/> の効果を受ける
    /// </summary>
    public class OchalikeBone : IReadOnlyOchalikeBone
    {
        public BoneKeyword Name { get; set; }
        public BoneSprite BareSprite { get; set; }
        public Color BareColor { get; set; }
        public bool OverridesOnDefaultColor { get; set; }
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
