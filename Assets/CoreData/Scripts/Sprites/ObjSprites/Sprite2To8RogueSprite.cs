using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;
using UnityEngine.Tilemaps;

namespace Roguegard
{
    public class Sprite2To8RogueSprite : RogueObjSprite
    {
        public override Sprite IconSprite => spriteLowerLeft;
        public override Color IconColor => color;

        private Sprite spriteLeft;
        private Sprite spriteLowerLeft;
        private Color color;

        private Sprite2To8RogueSprite()
        {
        }

        public static Sprite2To8RogueSprite CreateOrReuse(RogueObj self, Sprite spriteLowerLeft, Sprite spriteLeft, Color color)
        {
            var value = self.Main.Sprite.Sprite;
            if (value is Sprite2To8RogueSprite objSprite &&
                spriteLowerLeft == objSprite.spriteLowerLeft && spriteLeft == objSprite.spriteLeft && color == objSprite.color)
            {
                return objSprite;
            }
            else
            {
                var sprite8Sprite = CreateInstance<Sprite2To8RogueSprite>();
                sprite8Sprite.spriteLeft = spriteLeft;
                sprite8Sprite.spriteLowerLeft = spriteLowerLeft;
                sprite8Sprite.color = color;
                return sprite8Sprite;
            }
        }

        public override void SetTo(ISkeletalSpriteRenderController renderController, SpritePose pose, SpriteDirection direction)
        {
            var angleIndex = (int)direction;
            var sprite = angleIndex % 2 == 0 ? spriteLeft : spriteLowerLeft;
            var eulerZ = 180f + (angleIndex / 2) * 90f;
            var rotation = Quaternion.Euler(0f, 0f, eulerZ);

            renderController.AdjustBones(1);
            if (pose.BoneTransforms.TryGetValue(new BoneKeyword(BoneKw.Body.Name), out var transform))
            {
                renderController.SetBoneSprite(
                    0, BoneKw.Body.Name, sprite, color, false, false,
                    transform.LocalPosition, transform.LocalRotation * rotation, transform.ScaleOfLocalByLocal);
            }
            else
            {
                renderController.SetBoneSprite(
                    0, BoneKw.Body.Name, sprite, color, false, false, Vector3.zero, rotation, Vector3.one);
            }
        }

        public override void SetBoneSpriteEffects(RogueObj self, Spanning<IBoneSpriteEffect> effects)
        {
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = spriteLowerLeft;
            tileData.color = color;
        }
    }
}
