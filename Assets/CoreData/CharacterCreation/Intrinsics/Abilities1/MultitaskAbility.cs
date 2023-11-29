using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class MultitaskAbility : AbilityIntrinsicOptionScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(lv);
        }

        private class SortedIntrinsic : AbilitySortedIntrinsic, IRogueMethodPassiveAspect
        {
            float IRogueMethodPassiveAspect.Order => 0f;

            public SortedIntrinsic(int lv) : base(lv) { }

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveNext next)
            {
                var result = next.Invoke(keyword, method, self, user, activationDepth, arg);
                if (result && activationDepth < 1f && method is ISkill skill)
                {
                    var atk = skill.GetATK(self, out var additionalEffect);
                    if (atk <= 0 && additionalEffect)
                    {
                        // 攻撃スキルでなく、追加効果を持つとき通常攻撃する
                        default(IActiveRogueMethodCaller).NormalAttack(self, 1f, self.Position + self.Main.Stats.Direction.Forward);
                    }
                }
                return result;
            }
        }
    }
}
