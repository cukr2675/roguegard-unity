using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class MartialArtsAbility : AbilityIntrinsicOptionScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(lv);
        }

        private class SortedIntrinsic : AbilitySortedIntrinsic, IRogueMethodPassiveAspect
        {
            float IRogueMethodPassiveAspect.Order => 0f;

            private static readonly RogueMethodArgumentBuilder argumentBuilder = new RogueMethodArgumentBuilder();

            public SortedIntrinsic(int lv) : base(lv) { }

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveChain chain)
            {
                var result = chain.Invoke(keyword, method, self, user, activationDepth, arg);

                // 武器を使わずに通常攻撃するとき二回攻撃する
                if (keyword == MainInfoKw.Attack && method == self.Main.InfoSet.Attack)
                {
                    // 二回目の攻撃はターゲットを指定しない
                    argumentBuilder.SetArgument(arg);
                    argumentBuilder.TargetObj = null;

                    result = chain.Invoke(keyword, method, self, user, activationDepth, argumentBuilder.ToArgument());
                }

                return result;
            }
        }
    }
}
