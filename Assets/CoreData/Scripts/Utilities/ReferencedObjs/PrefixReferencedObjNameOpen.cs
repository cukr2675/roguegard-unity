using System.Collections;
using System.Collections.Generic;
using SkeletalSprite;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class PrefixReferencedObjNameOpen : ReferableScript, IOpenEffect
    {
        private static readonly StatusEffect statusEffect = new StatusEffect();

        public IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            RogueEffectUtility.AddFromInfoSet(self, statusEffect);
            return raceOption;
        }

        public void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            RogueEffectUtility.Remove(self, statusEffect);
        }

        public IRaceOption Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return raceOption;
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        private class StatusEffect : IStatusEffect
        {
            public string Name => null;
            public Sprite Icon => null;
            public Color Color => Color.white;
            public string Caption => null;
            public IRogueDetails Details => null;

            public IKeyword EffectCategory => EffectCategoryKw.Dummy;
            public RogueObj Effecter => null;
            public ISpriteMotion HeadIcon => null;
            public float Order => 0f;

            public void GetEffectedName(RogueNameBuilder refName, RogueObj self)
            {
                var infoSetVariantInfo = InfoSetReferenceInfo.Get(self);
                if (infoSetVariantInfo != null && infoSetVariantInfo.Count >= 1)
                {
                    var infoSet = infoSetVariantInfo.Get(0);
                    refName.Insert0(infoSet.Name);
                    return;
                }

                var tileVariantInfo = TileReferenceInfo.Get(self);
                if (tileVariantInfo != null && tileVariantInfo.Count >= 1)
                {
                    var tile = tileVariantInfo.Get(0);
                    refName.Insert0(tile.Info.Name);
                    return;
                }
            }
        }
    }
}
