using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class EffectStickerReference : RgpackReference<EffectStickerAsset>, IRogueEffect
    {
        public EffectStickerReference(string id, string envRgpackID)
            : base(id, envRgpackID)
        {
        }

        public void Open(RogueObj self)
        {
            Asset.Open(self);
        }

        public bool CanStack(RogueObj self, RogueObj comingObj, IRogueEffect coming)
        {
            return coming is EffectStickerReference effect && effect.FullID == FullID;
        }

        public IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            var clone = new EffectStickerReference(AssetID, RgpackID);
            return clone;
        }

        public IRogueEffect ReplaceObj(RogueObj obj, RogueObj clonedObj)
        {
            return this;
        }
    }
}
