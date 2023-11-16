using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;

namespace Roguegard
{
    public class ColoredRogueSprite : RogueObjSprite
    {
        public override Sprite IconSprite => sprite;
        public override Color IconColor => color;

        private Sprite sprite;
        private Color color;

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

        public override void SetTo(IRogueObjSpriteRenderController renderController, BonePose pose, RogueDirection direction)
        {
            renderController.AdjustBones(1);
            if (pose.BoneTransforms.TryGetValue(BoneKw.Body, out var transform))
            {
                var sprite = transform.Sprite != null ? transform.Sprite.NormalFront : this.sprite;
                var color = transform.OverridesSourceColor ? transform.Color : this.color;
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
