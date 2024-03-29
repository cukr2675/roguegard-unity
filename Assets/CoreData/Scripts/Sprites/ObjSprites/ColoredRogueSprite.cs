using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
using SkeletalSprite;

namespace Roguegard
{
    public class ColoredRogueSprite : IRogueObjSprite
    {
        private TileObject _tile;
        public TileBase Tile => _tile;

        public Color EffectedColor { get; private set; }

        private ColoredRogueSprite()
        {
        }

        public static ColoredRogueSprite Create(Sprite sprite, Color effectedColor)
        {
            var coloredSprite = new ColoredRogueSprite();
            coloredSprite._tile = ScriptableObject.CreateInstance<TileObject>();
            coloredSprite._tile.sprite = sprite;
            coloredSprite.EffectedColor = effectedColor;
            return coloredSprite;
        }

        public static ColoredRogueSprite CreateOrReuse(RogueObj self, Sprite sprite, Color effectedColor)
        {
            var value = self.Main.Sprite.Sprite;
            if (value is ColoredRogueSprite objSprite && sprite == objSprite._tile.sprite && effectedColor == objSprite.EffectedColor)
            {
                return objSprite;
            }
            else
            {
                return Create(sprite, effectedColor);
            }
        }

        public void SetTo(ISkeletalSpriteRenderController renderController, SpritePose pose, SpriteDirection direction)
        {
            renderController.AdjustBones(1);
            if (pose.BoneTransforms.TryGetValue(BoneKeyword.Body, out var transform))
            {
                var sprite = transform.Sprite != null ? transform.Sprite.NormalFront : _tile.sprite;
                var color = transform.OverridesSourceColor ? transform.Color : EffectedColor;
                renderController.SetBoneSprite(
                    0, BoneKeyword.Body.Name, sprite, color, transform.LocalMirrorX, transform.LocalMirrorY,
                    transform.LocalPosition, transform.LocalRotation, transform.ScaleOfLocalByLocal);
            }
            else
            {
                renderController.SetBoneSprite(
                    0, BoneKeyword.Body.Name, _tile.sprite, EffectedColor, false, false, Vector3.zero, Quaternion.identity, Vector3.one);
            }
        }

        public void SetBoneSpriteEffects(RogueObj self, Spanning<IBoneSpriteEffect> effects)
        {
        }

        private class TileObject : TileBase
        {
            public Sprite sprite;

            public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
            {
                tileData.sprite = sprite;
            }
        }
    }
}
