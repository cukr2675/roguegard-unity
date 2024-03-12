using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
using SkeletalSprite;

namespace Roguegard
{
    public class ColoredRogueSprite : RogueObjSprite
    {
        public override Sprite IconSprite => sprite;
        public override Color IconColor => color;

        private Sprite sprite;
        private Color32 color;

        private ColoredRogueSprite()
        {
        }

        public static ColoredRogueSprite Create(Sprite sprite, Color color)
        {
            var coloredSprite = CreateInstance<ColoredRogueSprite>();
            coloredSprite.sprite = sprite;
            coloredSprite.color = color;
            return coloredSprite;
        }

        public static ColoredRogueSprite CreateOrReuse(RogueObj self, Sprite sprite, Color color)
        {
            var value = self.Main.Sprite.Sprite;
            if (value is ColoredRogueSprite objSprite && sprite == objSprite.sprite && color == objSprite.color)
            {
                return objSprite;
            }
            else
            {
                return Create(sprite, color);
            }
        }

        public override void SetTo(IRogueObjSpriteRenderController renderController, BonePose pose, SpriteDirection direction)
        {
            renderController.AdjustBones(1);
            if (pose.BoneTransforms.TryGetValue(new BoneKeyword(BoneKw.Body.Name), out var transform))
            {
                var sprite = transform.Sprite != null ? transform.Sprite.NormalFront : IconSprite;
                var color = transform.OverridesSourceColor ? transform.Color : IconColor;
                renderController.SetBoneSprite(
                    0, BoneKw.Body.Name, sprite, color, transform.LocalMirrorX, transform.LocalMirrorY,
                    transform.LocalPosition, transform.LocalRotation, transform.ScaleOfLocalByLocal);
            }
            else
            {
                renderController.SetBoneSprite(
                    0, BoneKw.Body.Name, sprite, color, false, false, Vector3.zero, Quaternion.identity, Vector3.one);
            }
        }

        public override void SetBoneSpriteEffects(RogueObj self, Spanning<IBoneSpriteEffect> effects)
        {
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = sprite;
            tileData.color = color;
        }
    }
}
