using OchalikeSprites;
using UnityEngine;
using UnityEngine.Tilemaps;

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
            var tile = ScriptableObject.CreateInstance<TileObject>();
            tile.sprite = sprite;
            return new ColoredRogueSprite
            {
                _tile = tile,
                EffectedColor = effectedColor
            };
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

        public void SetTo(IOchalikeSpriteRenderController renderController, SpritePose pose, SpriteDirection direction)
        {
            renderController.AdjustBones(1);
            if (pose.BoneTransforms.TryGetValue(BoneKeyword.Body, out var transform))
            {
                var sprite = transform.PoseBareSprite != null ? transform.PoseBareSprite.NormalFront : _tile.sprite;
                var color = transform.PoseBareColor != null ? transform.PoseBareColor.Value : EffectedColor;
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
