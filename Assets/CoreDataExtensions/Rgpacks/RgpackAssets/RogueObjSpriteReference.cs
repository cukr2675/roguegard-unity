using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;
using UnityEngine.Tilemaps;

namespace Roguegard.Rgpacks
{
    public class RogueObjSpriteReference : RgpackReference<object>
    {
        private ObjSprite sprite;

        private RogueObjSpriteReference() { }

        public RogueObjSpriteReference(string id, string envRgpackID)
            : base(id, envRgpackID)
        {
        }

        private void Initialize()
        {
            if (Asset is CharacterCreationPresetAsset characterCreationPresetAsset)
            {
                var obj = characterCreationPresetAsset.LoadPreset().CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
                obj.Main.Sprite.Update(obj);
                sprite = new ObjSprite() { info = obj.Main.Sprite };
            }
            else if (Asset is SewedEquipmentData sewedEquipmentData)
            {
                var infoSet = new SewedEquipmentInfoSet(sewedEquipmentData);
                var obj = infoSet.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
                sprite = new ObjSprite() { info = obj.Main.Sprite };
            }
            else
            {
                throw new RogueException($"{FullID} ({Asset.GetType()}) をスプライトにできません。");
            }
        }

        public IRogueObjSprite GetObjSprite()
        {
            if (sprite == null) { Initialize(); }

            return sprite;
        }

        public ISpriteMotionSet GetMotionSet()
        {
            if (sprite == null) { Initialize(); }

            return sprite.info.MotionSet;
        }

        private class ObjSprite : IRogueObjSprite
        {
            public MainSpriteInfo info;

            public TileBase Tile => info.Sprite.Tile;
            public Color EffectedColor => info.Sprite.EffectedColor;

            public void SetBoneSpriteEffects(RogueObj self, Spanning<IBoneSpriteEffect> effects)
            {
            }

            public void SetTo(ISkeletalSpriteRenderController renderController, SpritePose pose, SpriteDirection direction)
            {
                info.SetTo(renderController, pose, direction);
            }
        }
    }
}
