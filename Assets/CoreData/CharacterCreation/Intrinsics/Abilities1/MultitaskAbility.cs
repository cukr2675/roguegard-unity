using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class MultitaskAbility : AbilityIntrinsicOption, IRogueMethodPassiveAspect
    {
        public override string Name => ":Multitask";
        protected override int Lv => 1;
        protected override float Cost => 3;

        float IRogueMethodPassiveAspect.Order => 0f;

        protected override void LevelUpToLv(RogueObj self, MainInfoSetType infoSetType)
        {
            base.LevelUpToLv(self, infoSetType);
        }

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
