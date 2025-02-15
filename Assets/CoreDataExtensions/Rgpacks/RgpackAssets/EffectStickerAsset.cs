using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OchalikeSprites;

namespace Roguegard.Rgpacks
{
    public class EffectStickerAsset : IRogueObjUpdater, IBoneSpriteEffect
    {
        private PropertiedCmnReference _update;
        float IRogueObjUpdater.Order => 0f;

        private PropertiedCmnReference _passive;

        private RogueObjSpriteReference _sprite;
        float IBoneSpriteEffect.Order => 0f;

        public EffectStickerAsset(EffectStickerInfo info, string envRgpackId, string fullId)
        {
            _update = info.Update.ToReference(envRgpackId);
            _sprite = new RogueObjSpriteReference(info.Sprite, envRgpackId);
        }

        public void Open(RogueObj self)
        {
            if (_update.Cmn.AssetExists)
            {
                var updaterState = self.Main.GetRogueObjUpdaterState(self);
                updaterState.AddFromRogueEffect(self, this);
            }
            if (_sprite.AssetExists)
            {
                var boneSpriteEffectState = self.Main.GetBoneSpriteEffectState(self);
                boneSpriteEffectState.AddFromRogueEffect(self, this);
            }
        }

        RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
        {
            _update.Invoke(self, null, activationDepth, RogueMethodArgument.Identity);
            return RogueObjUpdaterContinueType.Break;
        }

        void IBoneSpriteEffect.AffectSprite(RogueObj self, IReadOnlyOchalikeBone rootBone, OchalikeMorph ochalikeMorph)
        {
            _sprite.AddTo(ochalikeMorph);
        }
    }
}
