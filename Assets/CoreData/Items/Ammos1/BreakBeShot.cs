using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class BreakBeShot : BaseApplyRogueMethod
    {
        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => LineOfSight10RogueMethodRange.Instance;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RaycastAssert.BeThrown(
                LineOfSight10RogueMethodRange.Instance, self, user, arg,
                out var target, out var hitPosition, out var from, out var to, out var raycasted)) return false;
            if (raycasted)
            {
                MainCharacterWorkUtility.TryAddBeShot(self, user, hitPosition, from, CoreMotions.BeThrownFlying);
            }

            using var damageValue = EffectableValue.Get();
            if (arg.RefValue != null)
            {
                // 射撃武器から攻撃力を引き継ぐ。同じ RefValue が使いまわされることを想定してコピーを使用する
                arg.RefValue.CopyTo(damageValue);
            }
            else
            {
                // 引き継がない場合は攻撃力ダメージの攻撃。
                StatsEffectedValues.GetATK(user, damageValue);
            }
            var result = this.TryHurt(target, user, activationDepth, damageValue);
            this.TryDefeat(target, user, activationDepth, damageValue);

            if (result)
            {
                // 攻撃後は破損して消失する。
                self.TrySetStack(self.Stack - 1);
            }
            else
            {
                // 攻撃しなかった場合は下に落ちる。
                MainCharacterWorkUtility.TryAddBeDropped(self, user, to, CoreMotions.BeThrownDrop);

                // スタックしていたら一つだけ投げる。
                if (self.Stack >= 2) { self = SpaceUtility.Divide(self, 1); }
                this.Locate(self, null, user.Location, to, activationDepth); // user を null にしないと行動阻害で落とすことを阻害されてしまう
            }

            return true;
        }
    }
}
