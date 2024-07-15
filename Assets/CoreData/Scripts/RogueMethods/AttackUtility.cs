using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class AttackUtility
    {
        public static float BaseCupValueEffectOrder => -2f;
        public static float CupValueEffectOrder => -1f;

        /// <summary>
        /// 攻撃予定のターゲットと実際に攻撃するターゲットが一致しない場合、失敗させる。
        /// </summary>
        public static bool AssertTarget(RogueObj target, in RogueMethodArgument arg)
        {
            return arg.TargetObj != null && arg.TargetObj != target;
        }

        /// <summary>
        /// 空振り
        /// </summary>
        private static bool AssertWhiff(RogueObj target)
        {
            return target == null;
        }

        /// <summary>
        /// 二つの確率の和事象（少なくともどちらか発生する確率）を取得する。このメソッドを呼び出す <see cref="IValueEffect.Order"/> は
        /// <see cref="BaseCupValueEffectOrder"/> か <see cref="CupValueEffectOrder"/> にする。
        /// </summary>
        public static float Cup(float a, float b)
        {
            return a + b - a * b;
        }

        public static float GetActivationDepthCantCounter(float activationDepth)
        {
            return Mathf.Max(activationDepth, 1f);
        }

        public static RogueObj GetTargetForward(IProjectileRogueMethodRange range, RogueObj self)
        {
            var movement = MovementCalculator.Get(self);
            var direction = self.Main.Stats.Direction;
            range.Raycast(self.Location, self.Position, direction, true, movement.HasTileCollider, out var target, out _, out _);
            return target;
        }

        public static ISkill GetNormalAttackSkill(RogueObj self)
        {
            EquipmentUtility.GetWeapon(self, out var weapon);
            if (weapon?.Attack != null)
            {
                // 武器で攻撃する
                return weapon.Attack;
            }
            {
                // 素手で攻撃する
                return self.Main.InfoSet.Attack;
            }
        }

        public static bool GetUseValue(EffectableValue damageValue)
        {
            return damageValue != null ? damageValue.MainValue != 0f : false;
        }
    }
}
