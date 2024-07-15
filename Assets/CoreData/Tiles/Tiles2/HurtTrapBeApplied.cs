using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class HurtTrapBeApplied : BaseApplyRogueMethod
    {
        [SerializeField] private int _additionalDamage = 0;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var userTile = arg.Other as UserRogueTile;
            if (userTile != null && !StatsEffectedValues.AreVS(userTile.User, user))
            {
                // 敵対していないキャラが罠を踏んでも起動しないようにする
                return false;
            }

            if (userTile != null)
            {
                // 基礎攻撃力ぶんのダメージ
                using var damage = EffectableValue.Get();
                StatsEffectedValues.GetATK(userTile.User, damage);
                damage.Initialize(damage.BaseMainValue + _additionalDamage);
                var result = this.Hurt(user, userTile.User, AttackUtility.GetActivationDepthCantCounter(activationDepth), damage);
                this.TryDefeat(user, userTile.User, activationDepth, damage);
                return result;
            }
            else
            {
                // 1 のダメージ
                using var damage = EffectableValue.Get();
                damage.Initialize(1 + _additionalDamage);
                var result = this.Hurt(user, null, AttackUtility.GetActivationDepthCantCounter(activationDepth), damage);
                this.TryDefeat(user, null, activationDepth, damage);
                return result;
            }
        }
    }
}
