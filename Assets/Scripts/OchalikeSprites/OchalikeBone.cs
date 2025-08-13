using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    /// <summary>
    /// <see cref="OchalikeSpriteAsset"/> を構成するボーン。
    /// これをもとに <see cref="OchalikeMorph"/> と <see cref="SpritePose"/> の効果を受ける
    /// </summary>
    public class OchalikeBone : IReadOnlyOchalikeBone
    {
        public BoneKeyword Name { get; set; }
        public BoneSprite BareSprite { get; set; }
        public Color BareColor { get; set; } = Color.white;
        public bool OverridesOnDefaultColor { get; set; }
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }
        public Vector3 LocalPosition { get; set; } = Vector3.zero;
        public Quaternion LocalRotation { get; set; } = Quaternion.identity;
        public Vector3 ScaleOfLocalByLocal { get; set; } = Vector3.one;
        public float NormalOrderInParent { get; set; }
        public float BackOrderInParent { get; set; }

        public OchalikeBoneList Children { get; } = new OchalikeBoneList();

        IReadOnlyList<IReadOnlyOchalikeBone> IReadOnlyOchalikeBone.Children => Children;

        public static OchalikeBone CreateClearOchalikeSprite(Color bareColor)
        {
            var clearSprite = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.zero);
            return new OchalikeBone
            {
                Name = BoneKeyword.Body,
                BareSprite = BoneSprite.CreateNFBR_NRBF(clearSprite, clearSprite),
                BareColor = bareColor,
                OverridesOnDefaultColor = true
            };
        }
    }
}
