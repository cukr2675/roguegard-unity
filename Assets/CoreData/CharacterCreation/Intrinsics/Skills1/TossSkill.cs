using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class TossSkill : MPSkill
    {
        public override string Name => "放り投げる";
        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => LineOfSight10RogueMethodRange.Instance;
        public override int RequiredMP => 2;

        protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RaycastAssert.RequireTarget(FrontRogueMethodRange.Instance, self, arg, out var target)) return false;
            if (MainCharacterWorkUtility.TryAddAttack(self))
            {
                RogueDevice.Add(DeviceKw.AppendText, self);
                RogueDevice.Add(DeviceKw.AppendText, "は");
                RogueDevice.Add(DeviceKw.AppendText, target);
                RogueDevice.Add(DeviceKw.AppendText, "を放り投げた！\n");
            }

            var angle = RogueRandom.Primary.Next(0, 8);
            var direction = new RogueDirection(angle);
            ((IProjectileRogueMethodRange)LineOfSight10RogueMethodRange.Instance).Raycast(
                self.Location, self.Position, direction, true, true, out var hitObj, out var hitPosition, out var dropPosition);
            if (hitPosition != dropPosition)
            {
                // 壁に当たったら1ダメージ
                using var damageValue = AffectableValue.Get();
                damageValue.Initialize(1f);
                this.TryHurt(target, self, activationDepth, damageValue);
                this.TryDefeat(target, self, activationDepth, damageValue);
            }
            if (hitObj != null)
            {
                // 何かに当たったら当たったオブジェクトに1ダメージ
                using var damageValue = AffectableValue.Get();
                damageValue.Initialize(1f);
                this.TryHurt(target, self, activationDepth, damageValue);
                this.TryDefeat(target, self, activationDepth, damageValue);
            }
            return true;
        }

        public override int GetATK(RogueObj self, out bool additionalEffect)
        {
            // 攻撃力+2ダメージの攻撃
            using var damageValue = AffectableValue.Get();
            StatsEffectedValues.GetATK(self, damageValue);
            damageValue.MainValue += 2;
            var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
            additionalEffect = false;
            return hpDamage;
        }
    }
}
