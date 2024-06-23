using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class BareHandsAbility : AbilityIntrinsicOptionScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(lv);
        }

        private class SortedIntrinsic : AbilitySortedIntrinsic, IRogueMethodPassiveAspect, IValueEffect
        {
            [System.NonSerialized] private int buffCount;

            float IRogueMethodPassiveAspect.Order => 0f;
            float IValueEffect.Order => 0f;

            public SortedIntrinsic(int lv) : base(lv) { }

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveChain chain)
            {
                // 武器を使わずに通常攻撃するとき会心率 +50%
                var turnOn = keyword == MainInfoKw.Attack && method == self.Main.InfoSet.Attack;
                if (turnOn) { buffCount++; }

                var result = chain.Invoke(keyword, method, self, user, activationDepth, arg);
                if (turnOn) { buffCount--; }
                return result;
            }

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (buffCount >= 1 && keyword == StatsKw.ATK)
                {
                    value.SubValues[StatsKw.CriticalRate] += .5f;
                }
            }
        }
    }
}
