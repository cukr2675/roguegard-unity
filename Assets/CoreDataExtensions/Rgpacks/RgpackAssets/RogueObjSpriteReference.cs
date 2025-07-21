using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OchalikeSprites;
using UnityEngine.Tilemaps;
using Roguegard.CharacterCreation;

namespace Roguegard.Rgpacks
{
    public class RogueObjSpriteReference : RgpackReference<object>
    {
        private ObjSprite sprite;
        private OchalikeMorph ochalikeMorph;

        private RogueObjSpriteReference() { }

        public RogueObjSpriteReference(string id, string envRgpackId)
            : base(id, envRgpackId)
        {
        }

        private void Initialize()
        {
            var random = new RogueRandom(0);
            if (Asset is CharacterCreationPresetAsset characterCreationPresetAsset)
            {
                var obj = characterCreationPresetAsset.LoadPreset().CreateObj(null, Vector2Int.zero, random);
                obj.Main.Sprite.Update(obj);
                sprite = new ObjSprite() { info = obj.Main.Sprite };
            }
            else if (Asset is SewedEquipmentData sewedEquipmentData)
            {
                var infoSet = new SewedEquipmentInfoSet(sewedEquipmentData);
                var obj = infoSet.CreateObj(null, Vector2Int.zero);
                sprite = new ObjSprite() { info = obj.Main.Sprite };
                ochalikeMorph = sewedEquipmentData.BoneSprites.GetOchalikeMorph();
            }
            else if (Asset is RaceOptionalCreationData raceOptionalCreationData)
            {
                var obj = raceOptionalCreationData.CreateObj(null, Vector2Int.zero, random);
                obj.Main.Sprite.Update(obj);
                sprite = new ObjSprite() { info = obj.Main.Sprite };
            }
            else
            {
                throw new RogueException($"{FullId} ({Asset.GetType()}) をスプライトにできません。");
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

        public void AddTo(OchalikeMorph ochalikeMorph)
        {
            if (sprite == null) { Initialize(); }
            if (this.ochalikeMorph == null) throw new System.InvalidOperationException();

            this.ochalikeMorph.AddTo(ochalikeMorph);
        }

        private class ObjSprite : IRogueObjSprite
        {
            public MainSpriteInfo info;

            public TileBase Tile => info.Sprite.Tile;
            public Color EffectedColor => info.Sprite.EffectedColor;

            public void SetBoneSpriteEffects(RogueObj self, Spanning<IBoneSpriteEffect> effects)
            {
            }

            public void SetTo(IOchalikeSpriteRenderController renderController, SpritePose pose, SpriteDirection direction)
            {
                info.SetTo(renderController, pose, direction);
            }
        }
    }
}
