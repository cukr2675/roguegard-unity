using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class TossAttackSkill : MPSkill
    {
        public override string Name => "ぶん投げる";
        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => LineOfSight10RogueMethodRange.Instance;
        public override int RequiredMP => 2;

        protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            using var predicator = ForAllRogueMethodTarget.Instance.GetPredicator(self, 0f, null);
            Within1TileRogueMethodRange.Instance.Predicate(predicator, self, 0f, null, self.Position);
            var objs = predicator.GetObjs(self.Position);
            if (objs.Count >= 1)
            {
                if (RogueDevice.Primary.Player == self)
                {
                    RogueDevice.Add(DeviceKw.AppendText, "投げるものが見つからない\n");
                }
                return false;
            }
            if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                using var handler = h;
                handler.AppendText(self).AppendText("は").AppendText(objs[0]).AppendText("をぶん投げた！\n");
            }

            var direction = RogueMethodUtility.GetTargetDirection(self, arg);
            ((IProjectileRogueMethodRange)LineOfSight10RogueMethodRange.Instance).Raycast(
                self.Location, self.Position, direction, true, true, out var hitObj, out var hitPosition, out var dropPosition);

            // 攻撃力+2ダメージの攻撃
            using var damageValue = AffectableValue.Get();
            StatsEffectedValues.GetATK(self, damageValue);
            damageValue.MainValue += 2;
            //this.TryHurt(target, self, activationDepth, damageValue);
            //this.TryDefeat(target, self, activationDepth, damageValue);
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
