using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Extensions
{
    public static class CoreRogueMethodExtension
    {
        private static readonly LoseExpRogueMethod loseExp = new LoseExpRogueMethod();
        private static readonly TryAnyRogueMethod tryAnyRogueMethod = new TryAnyRogueMethod();
        private static readonly TryAnyKeyword tryAnyKeyword = new TryAnyKeyword();

        public static bool PutIn(
            this IApplyRogueMethodCaller method, RogueObj self, RogueObj chest, IChestInfo chestInfo, RogueObj obj, float activationDepth)
        {
            var arg = new RogueMethodArgument(targetObj: obj);
            return RogueMethodAspectState.Invoke(MainInfoKw.Walk, chestInfo.TakeIn, chest, self, activationDepth, arg);
        }

        public static bool TakeOut(
            this IApplyRogueMethodCaller method, RogueObj self, RogueObj chest, IChestInfo chestInfo, RogueObj obj, float activationDepth)
        {
            var arg = new RogueMethodArgument(targetObj: obj);
            return RogueMethodAspectState.Invoke(MainInfoKw.Walk, chestInfo.PutOut, chest, self, activationDepth, arg);
        }

        public static bool Walk(
            this IActiveRogueMethodCaller method, RogueObj self, RogueDirection direction, float activationDepth)
        {
            var walkMethod = self.Main.InfoSet.Walk;
            var arg = new RogueMethodArgument(targetPosition: self.Position + direction.Forward);
            return RogueMethodAspectState.Invoke(MainInfoKw.Walk, walkMethod, self, null, activationDepth, arg);
        }

        public static bool NormalAttack(
            this IActiveRogueMethodCaller method, RogueObj self, float activationDepth, Vector2Int targetPosition, RogueObj target = null)
        {
            var arg = new RogueMethodArgument(targetPosition: targetPosition, targetObj: target);

            EquipmentUtility.GetWeapon(self, out var weapon);
            if (weapon?.Attack != null)
            {
                // 武器で攻撃する。
                var result = RogueMethodAspectState.Invoke(MainInfoKw.Attack, weapon.Attack, self, null, activationDepth, arg);
                if (result) return true;
            }
            {
                // 素手で攻撃する。
                var skill = self.Main.InfoSet.Attack;
                var result = RogueMethodAspectState.Invoke(MainInfoKw.Attack, skill, self, null, activationDepth, arg);
                if (result) return true;
            }
            return false;
        }

        public static bool Eat(
            this IApplyRogueMethodCaller method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var beEatenMethod = self.Main.InfoSet.BeEaten;
            return RogueMethodAspectState.Invoke(MainInfoKw.BeEaten, beEatenMethod, self, user, activationDepth, arg);
        }

        public static bool StepOn(this IApplyRogueMethodCaller method, RogueObj self, float activationDepth)
        {
            SpaceUtility.GetTop(self.Location.Space, self.Position, out var tileObj, out var tile);

            // 割り込み回数を減らすため、トラップ層のタイルだけを踏む
            if (tile != null && tile.Info.Layer == RogueTileLayer.Building)
            {
                var arg = new RogueMethodArgument(other: tile);
                return RogueMethodAspectState.Invoke(
                    StdKw.StepOn, tile.Info.BeApplied, null, self, activationDepth, arg);
            }
            else if (tileObj != null && tileObj.AsTile && tileObj.HasTileCollider)
            {
                return RogueMethodAspectState.Invoke(
                    StdKw.StepOn, tileObj.Main.InfoSet.BeApplied, tileObj, self, activationDepth, RogueMethodArgument.Identity);
            }
            return false;
        }

        public static bool Throw(
            this IApplyRogueMethodCaller method, RogueObj ammo, RogueObj self, float activationDepth,
            RogueObj targetObj = null, AffectableValue damageValue = null)
        {
            var beThrownMethod = ammo.Main.InfoSet.BeThrown;
            var arg = new RogueMethodArgument(targetObj: targetObj, value: damageValue);
            return RogueMethodAspectState.Invoke(MainInfoKw.BeThrown, beThrownMethod, ammo, self, activationDepth, arg);
        }

        public static bool Throw(
            this IApplyRogueMethodCaller method, RogueObj ammo, RogueObj self, float activationDepth,
            Vector2Int targetPosition, RogueObj targetObj = null, AffectableValue damageValue = null)
        {
            var beThrownMethod = ammo.Main.InfoSet.BeThrown;
            var arg = new RogueMethodArgument(targetPosition: targetPosition, targetObj: targetObj, value: damageValue);
            return RogueMethodAspectState.Invoke(MainInfoKw.BeThrown, beThrownMethod, ammo, self, activationDepth, arg);
        }

        /// <summary>
        /// <paramref name="targetObj"/> が指定されているとき、当たり判定と飛ばして <paramref name="targetObj"/> に直接攻撃する。
        /// <paramref name="targetObj"/> が指定されていないときは <paramref name="self"/> の前方へ発射する。（または位置指定 UI を表示する？）
        /// </summary>
        public static bool Shot(
            this IApplyRogueMethodCaller method, IAmmoEquipmentInfo ammoInfo, RogueObj ammo, RogueObj self, float activationDepth,
            RogueObj targetObj = null, AffectableValue damageValue = null)
        {
            var beShotMethod = ammoInfo.BeShot;
            var arg = new RogueMethodArgument(targetObj: targetObj, value: damageValue);
            return RogueMethodAspectState.Invoke(StdKw.BeShot, beShotMethod, ammo, self, activationDepth, arg);
        }

        /// <summary>
        /// <paramref name="targetPosition"/> と <paramref name="targetObj"/> が共に指定されているとき、
        /// 一発目が <paramref name="targetObj"/> に命中しない場合は失敗させる。
        /// <paramref name="targetPosition"/> だけ指定されているときは失敗させない。
        /// </summary>
        public static bool Shot(
            this IApplyRogueMethodCaller method, IAmmoEquipmentInfo ammoInfo, RogueObj ammo, RogueObj self, float activationDepth,
            Vector2Int targetPosition, RogueObj targetObj = null, AffectableValue damageValue = null)
        {
            var beShotMethod = ammoInfo.BeShot;
            var arg = new RogueMethodArgument(targetPosition: targetPosition, targetObj: targetObj, value: damageValue);
            return RogueMethodAspectState.Invoke(StdKw.BeShot, beShotMethod, ammo, self, activationDepth, arg);
        }

        public static bool Hurt(
            this IAffectRogueMethodCaller method, RogueObj target, RogueObj user, float activationDepth, AffectableValue damageValue)
        {
            var hitMethod = target.Main.InfoSet.Hit;
            var arg = new RogueMethodArgument(value: damageValue);
            var damageResult = RogueMethodAspectState.Invoke(MainInfoKw.Hit, hitMethod, target, user, activationDepth, arg);
            return damageResult;
        }

        public static bool TryHurt(
            this IAffectRogueMethodCaller method, RogueObj target, RogueObj user, float activationDepth, AffectableValue damageValue)
        {
            if (target != null) return method.Hurt(target, user, activationDepth, damageValue);
            else return false;
        }

        public static bool Affect(
            this IAffectRogueMethodCaller method, RogueObj target, float activationDepth, IAffectCallback callback,
            RogueObj tool = null, RogueObj user = null, AffectableValue refValue = null)
        {
            var hitMethod = target.Main.InfoSet.Hit;
            var arg = new RogueMethodArgument(tool: tool, other: callback, value: refValue);
            return RogueMethodAspectState.Invoke(MainInfoKw.Hit, hitMethod, target, user, activationDepth, arg);
        }

        public static bool TryAffect(
            this IAffectRogueMethodCaller method, RogueObj target, float activationDepth, IAffectCallback callback,
            RogueObj tool = null, RogueObj user = null, AffectableValue refValue = null)
        {
            if (target != null) return method.Affect(target, activationDepth, callback, tool, user, refValue);
            else return false;
        }

        public static bool Defeat(
            this IAffectRogueMethodCaller method, RogueObj target, RogueObj user, float activationDepth)
        {
            var beDefeatedMethod = target.Main.InfoSet.BeDefeated;
            var defeatResult = RogueMethodAspectState.Invoke(
                MainInfoKw.BeDefeated, beDefeatedMethod, target, user, activationDepth, RogueMethodArgument.Identity);
            if (!defeatResult) return false;

            if (user != null)
            {
                using var expValue = AffectableValue.Get();
                expValue.Initialize(target.Main.Stats.Lv);
                ValueEffectState.AffectValue(StdKw.LoseExp, expValue, target);

                var addExpArg = new RogueMethodArgument(value: expValue);
                RogueMethodAspectState.Invoke(StdKw.LoseExp, loseExp, target, user, activationDepth, addExpArg);
            }
            return true;
        }

        public static bool TryDefeat(
            this IAffectRogueMethodCaller method, RogueObj target, RogueObj user, float activationDepth, AffectableValue damageValue)
        {
            if (target != null && damageValue.SubValues[MainInfoKw.BeDefeated] != 0f)
            {
                return method.Defeat(target, user, activationDepth);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// <paramref name="user"/> が行動できるか確かめる。
        /// </summary>
        public static bool TryAny(this IAffectRogueMethodCaller method, RogueObj user)
        {
            return RogueMethodAspectState.Invoke(
                tryAnyKeyword, tryAnyRogueMethod, null, user, Mathf.Infinity, RogueMethodArgument.Identity);
        }

        private class LoseExpRogueMethod : IChangeStateRogueMethod
        {
            public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                var exp = Mathf.FloorToInt(arg.RefValue.MainValue);
                if (RogueDevice.Primary.Player.Main.Stats.Party.Members.Contains(user))
                {
                    // レベルアップメッセージより先に表示する
                    RogueDevice.Add(DeviceKw.AppendText, user);
                    RogueDevice.Add(DeviceKw.AppendText, "は");
                    RogueDevice.Add(DeviceKw.AppendText, exp);
                    RogueDevice.Add(DeviceKw.AppendText, "の経験値を得た\n");
                }
                user.Main.Stats.AddExp(user, exp);
                return true;
            }
        }

        private class TryAnyRogueMethod : IAffectRogueMethod
        {
            public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                return true;
            }
        }

        private class TryAnyKeyword : IKeyword
        {
            public string Name => "Any";
            public Sprite Icon => null;
            public Color Color => Color.white;
            public string Caption => null;
            public object Details => null;
        }
    }
}
