using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;
using UnityEngine.Tilemaps;

namespace Roguegard
{
    public class Sprite2To8RogueSprite : IRogueObjSprite
    {
        private Sprite spriteLeft;
        private Sprite spriteLowerLeft;

        private TileObject _tile;
        public TileBase Tile => _tile;

        public Color EffectedColor { get; private set; }

        private Sprite2To8RogueSprite()
        {
        }

        public static Sprite2To8RogueSprite CreateOrReuse(RogueObj self, Sprite spriteLowerLeft, Sprite spriteLeft, Color effectedColor)
        {
            var value = self.Main.Sprite.Sprite;
            if (value is Sprite2To8RogueSprite objSprite &&
                spriteLowerLeft == objSprite.spriteLowerLeft && spriteLeft == objSprite.spriteLeft && effectedColor == objSprite.EffectedColor)
            {
                return objSprite;
            }
            else
            {
                var sprite8Sprite = new Sprite2To8RogueSprite();
                sprite8Sprite.spriteLeft = spriteLeft;
                sprite8Sprite.spriteLowerLeft = spriteLowerLeft;
                sprite8Sprite._tile = ScriptableObject.CreateInstance<TileObject>();
                sprite8Sprite._tile.sprite = spriteLowerLeft;
                sprite8Sprite.EffectedColor = effectedColor;
                return sprite8Sprite;
            }
        }

        public void SetTo(ISkeletalSpriteRenderController renderController, SpritePose pose, SpriteDirection direction)
        {
            var angleIndex = (int)direction;
            var sprite = angleIndex % 2 == 0 ? spriteLeft : spriteLowerLeft;
            var eulerZ = 180f + (angleIndex / 2) * 90f;
            var rotation = Quaternion.Euler(0f, 0f, eulerZ);

            renderController.AdjustBones(1);
            if (pose.BoneTransforms.TryGetValue(BoneKeyword.Body, out var transform))
            {
                renderController.SetBoneSprite(
                    0, BoneKeyword.Body.Name, sprite, EffectedColor, false, false,
                    transform.LocalPosition, transform.LocalRotation * rotation, transform.ScaleOfLocalByLocal);
            }
            else
            {
                renderController.SetBoneSprite(
                    0, BoneKeyword.Body.Name, sprite, EffectedColor, false, false, Vector3.zero, rotation, Vector3.one);
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
