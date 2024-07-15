using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;

namespace Roguegard
{
    public static class MainCharacterWorkUtility
    {
        public static void EnqueueViewDequeueState(RogueObj obj)
        {
            if (ViewInfo.TryGet(obj, out var view) && !view.QueueHasItem)
            {
                view.ReadyView(obj.Location);
                view.AddView(obj);
                view.EnqueueState();
                RogueDevice.Add(DeviceKw.EnqueueViewDequeueState, 0);
            }
        }

        public static bool TryAddSkill(RogueObj self)
        {
            if (!MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var handler)) return false;

            handler.EnqueueSE(MainInfoKw.Skill);
            handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(self, CoreMotions.FullTurn, false));
            handler.Dispose();
            return true;
        }

        /// <summary>
        /// 攻撃モーションをプッシュする
        /// </summary>
        public static bool TryAddAttack(RogueObj self)
        {
            if (!MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var handler)) return false;

            handler.EnqueueSE(MainInfoKw.Attack);
            handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(self, KeywordSpriteMotion.Attack, false));
            handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(self, KeywordSpriteMotion.Wait, true));
            handler.Dispose();
            return true;
        }

        public static bool TryAddShot(RogueObj self)
        {
            if (!MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var handler)) return false;

            handler.EnqueueSE(StdKw.GunThrow);
            handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(self, KeywordSpriteMotion.GunThrow, false));
            handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(self, KeywordSpriteMotion.Wait, true));
            handler.Dispose();
            return true;
        }

        /// <summary>
        /// オブジェクトが発射されるモーションをプッシュする
        /// </summary>
        public static bool TryAddBeThrown(RogueObj ammo, RogueObj user, Vector2Int hitPosition, Vector2Int from, ISpriteMotion motion)
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

        public static bool TryAddBeShot(RogueObj ammo, RogueObj user, Vector2Int hitPosition, Vector2Int from, ISpriteMotion motion)
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
                if (MessageWorkListener.CanHandle(location, position)) return true;
            }
            position = default;
            return false;
        }

        /// <summary>
        /// 発射したオブジェクトが地面に落ちるモーションをプッシュする
        /// </summary>
        public static bool TryAddBeDropped(RogueObj ammo, RogueObj user, Vector2Int to, ISpriteMotion motion)
        {
            if (!MessageWorkListener.CanHandle(user.Location, to)) return false;

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
            else if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                using var handler = h;
                handler.EnqueueWork(RogueCharacterWork.CreateWalk(self, self.Position, self.Main.Stats.Direction, null, true));
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
