using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard;

namespace RoguegardUnity
{
    public class RogueTickerTesterOpen : ReferableScript, IOpenEffect
    {
        [SerializeField] private LocateType _locateType;

        private Effect effect;

        public IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            effect ??= new Effect() { parent = this };

            RogueEffectUtility.AddFromInfoSet(self, effect);
            return raceOption;
        }

        public void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            effect ??= new Effect() { parent = this };

            RogueEffectUtility.Remove(self, effect);
        }

        public IRaceOption Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return raceOption;
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        private class Effect : IRogueObjUpdater
        {
            public float Order => 100f;

            public RogueTickerTesterOpen parent;
            public int counter;

            public RogueObjUpdaterContinueType UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                if (counter >= 100)
                {
                    Debug.LogError("無限再帰を検出しました");
                    RogueDevice.Add(null, 0f);
                    return default;
                }
                counter++;

                var clone = self.Clone(true);
                var location = parent._locateType == LocateType.LocateSelf ? self : self.Location;
                if (!SpaceUtility.TryLocate(clone, location))
                {
                    Debug.LogError($"クローン ({clone}) の移動に失敗しました。");
                    return default;
                }

                return default;
            }
        }

        private enum LocateType
        {
            LocateSelf,
            LocateLocation
        }
    }
}
