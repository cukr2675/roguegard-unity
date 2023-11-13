using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class MainCharacterWorkUtility
    {
        public static void EnqueueViewDequeueState(RogueObj obj)
        {
            if (obj.TryGet<ViewInfo>(out var view) && !view.QueueHasItem)
            {
                view.ReadyView(obj.Location);
                view.AddView(obj);
                view.EnqueueState();
                RogueDevice.Add(DeviceKw.EnqueueViewDequeueState, 0);
            }
        }

        public static bool TryAddSkill(RogueObj self)
        {
            if (!RogueDevice.Primary.VisibleAt(self.Location, self.Position)) return false;

            RogueDevice.Add(DeviceKw.EnqueueSE, MainInfoKw.Skill);
            var item = RogueCharacterWork.CreateBoneMotion(self, CoreMotions.FullTurn, false);
            RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
            return true;
        }

        /// <summary>
        /// 攻撃モーションをプッシュする
        /// </summary>
        public static bool TryAddAttack(RogueObj self)
        {
            if (!RogueDevice.Primary.VisibleAt(self.Location, self.Position)) return false;

            RogueDevice.Add(DeviceKw.EnqueueSE, MainInfoKw.Attack);
            var item = RogueCharacterWork.CreateBoneMotion(self, KeywordBoneMotion.Attack, false);
            RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
            item = RogueCharacterWork.CreateBoneMotion(self, KeywordBoneMotion.Wait, true);
            RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
            return true;
        }

        public static bool TryAddShot(RogueObj self)
        {
            if (!RogueDevice.Primary.VisibleAt(self.Location, self.Position)) return false;

            RogueDevice.Add(DeviceKw.EnqueueSE, StdKw.GunThrow);
            var item = RogueCharacterWork.CreateBoneMotion(self, KeywordBoneMotion.GunThrow, false);
            RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
            item = RogueCharacterWork.CreateBoneMotion(self, KeywordBoneMotion.Wait, true);
            RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
            return true;
        }

        /// <summary>
        /// オブジェクトが発射されるモーションをプッシュする
        /// </summary>
        public static bool TryAddBeThrown(RogueObj ammo, RogueObj user, Vector2Int hitPosition, Vector2Int from, IBoneMotion motion)
        {
            if (!TryGetLineVisiblePosition(user.Location, from, hitPosition, out var startPosition) ||
                !TryGetLineVisiblePosition(user.Location, hitPosition, from, out var endPosition)) return false;

            RogueDevice.Add(DeviceKw.EnqueueSE, MainInfoKw.BeThrown);
            var direction = RogueDirection.FromSignOrLowerLeft(endPosition - startPosition);
            var beforeItem = RogueCharacterWork.CreateWalk(ammo, startPosition, Mathf.Infinity, direction, motion, false);
            var item = RogueCharacterWork.CreateWalk(ammo, endPosition, 8f, direction, motion, false);
            RogueDevice.AddWork(DeviceKw.EnqueueWork, beforeItem);
            RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
            return true;
        }

        public static bool TryAddBeShot(RogueObj ammo, RogueObj user, Vector2Int hitPosition, Vector2Int from, IBoneMotion motion)
        {
            if (!TryGetLineVisiblePosition(user.Location, from, hitPosition, out var startPosition) ||
                !TryGetLineVisiblePosition(user.Location, hitPosition, from, out var endPosition)) return false;

            RogueDevice.Add(DeviceKw.EnqueueSE, StdKw.BeShot);
            var direction = RogueDirection.FromSignOrLowerLeft(endPosition - startPosition);
            var beforeItem = RogueCharacterWork.CreateWalk(ammo, startPosition, Mathf.Infinity, direction, motion, false);
            var item = RogueCharacterWork.CreateWalk(ammo, endPosition, 8f, direction, motion, false);
            RogueDevice.AddWork(DeviceKw.EnqueueWork, beforeItem);
            RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
            return true;
        }

        private static bool TryGetLineVisiblePosition(RogueObj location, Vector2Int p1, Vector2Int p2, out Vector2Int position)
        {
            var distance = Mathf.Ceil((p2 - p1).magnitude);
            for (int i = 0; i <= distance; i++)
            {
                var x = Mathf.FloorToInt(Mathf.Lerp(p1.x, p2.x, i / distance));
                var y = Mathf.FloorToInt(Mathf.Lerp(p1.y, p2.y, i / distance));
                position = new Vector2Int(x, y);
                if (RogueDevice.Primary.VisibleAt(location, position)) return true;
            }
            position = default;
            return false;
        }

        /// <summary>
        /// 発射したオブジェクトが地面に落ちるモーションをプッシュする
        /// </summary>
        public static bool TryAddBeDropped(RogueObj ammo, RogueObj user, Vector2Int to, IBoneMotion motion)
        {
            if (!RogueDevice.Primary.VisibleAt(user.Location, to)) return false;

            var drop = RogueCharacterWork.CreateWalk(ammo, to, -.5f, ammo.Main.Stats.Direction, motion, false);
            RogueDevice.AddWork(DeviceKw.EnqueueWork, drop);
            return true;
        }

        /// <summary>
        /// <paramref name="self"/> がプレイヤーキャラだった場合、 <see cref="RogueCharacterWork.Continues"/> == false のアニメーションを追加する。
        /// 方向転換直後に RogueDevice.Next() を呼び出すことでダッシュボタンの絵柄をすぐに変えるのが目的。
        /// </summary>
        public static bool TryAddTurn(RogueObj self)
        {
            if (RogueDevice.Primary.Player == self)
            {
                var work = RogueCharacterWork.CreateSyncPositioning(self);
                RogueDevice.AddWork(DeviceKw.EnqueueWork, work);
                return true;
            }
            else if (RogueDevice.Primary.VisibleAt(self.Location, self.Position))
            {
                var work = RogueCharacterWork.CreateWalk(self, self.Position, self.Main.Stats.Direction, null, true);
                RogueDevice.AddWork(DeviceKw.EnqueueWork, work);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
