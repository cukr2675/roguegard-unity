using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class SetSPDOpen : ReferableScript, IOpenEffect
    {
        [SerializeField] private int _spd = 0;

        private Effect effect;

        private SetSPDOpen() { }

        public IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            if (effect == null)
            {
                effect = new Effect();
                effect.spd = _spd;
            }

            RogueEffectUtility.AddFromInfoSet(self, effect);
            SpeedCalculator.SetDirty(self);
            return raceOption;
        }

        public void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            RogueEffectUtility.Remove(self, effect);
            SpeedCalculator.SetDirty(self);
        }

        public IRaceOption Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return raceOption;
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        private class Effect : IValueEffect
        {
            public int spd;

            public float Order => -100f;

            public void AffectValue(IKeyword keyword, EffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.Speed)
                {
                    value.MainValue += spd;
                }
            }
        }
    }
}
