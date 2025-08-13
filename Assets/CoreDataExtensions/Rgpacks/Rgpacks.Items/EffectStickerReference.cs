namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class EffectStickerReference : RgpackReference<EffectStickerAsset>, IRogueEffect
    {
        public EffectStickerReference(string id, string envRgpackId)
            : base(id, envRgpackId)
        {
        }

        public void Open(RogueObj self)
        {
            Asset.Open(self);
        }

        public bool CanStack(RogueObj self, RogueObj comingObj, IRogueEffect coming)
        {
            return coming is EffectStickerReference effect && effect.FullId == FullId;
        }

        public IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            var clone = new EffectStickerReference(AssetId, RgpackId);
            return clone;
        }

        public IRogueEffect ReplaceObj(RogueObj obj, RogueObj clonedObj)
        {
            return this;
        }
    }
}
