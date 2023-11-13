using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class RaycastAssert
    {
        public static bool BeThrown(
            IProjectileRogueMethodRange range, RogueObj ammo, RogueObj user, in RogueMethodArgument arg,
            out RogueObj hitObj, out Vector2Int hitPosition, out Vector2Int from, out Vector2Int to, out bool raycasted)
        {
            var ammoMovement = MovementCalculator.Get(ammo);
            var space = user.Location;
            RogueDirection direction;
            if (arg.TargetObj != null && !arg.TryGetTargetPosition(out _))
            {
                // TargetObj が指定されていてかつ TargetPosition が指定されていなかった場合、対象オブジェクトへ直接ヒット。
                hitObj = arg.TargetObj;
                hitPosition = hitObj.Position;
                from = user.Position;
                to = hitPosition;
                raycasted = false;
                if (!RogueDirection.TryFromSign(to - from, out direction)) return true;
            }
            else
            {
                // そうでない場合、前方の当たり判定を確認する。
                from = user.Position;
                direction = RogueMethodUtility.GetTargetDirection(user, arg);
                range.Raycast(space, from, direction, true, ammoMovement.HasTileCollider, out hitObj, out hitPosition, out to);
                raycasted = true;
            }

            if (ammoMovement.HasCollider) // HasCollider == true は壁などに重ならないように落とす
            {
                // キャラがオブジェクトに当たったとき、オブジェクトの手前に落とす。
                if (hitObj != null) { to -= direction.Forward; }

                var dropPositionObj = space.Space.GetColliderObj(to);
                if (dropPositionObj != null && dropPositionObj != ammo && dropPositionObj.HasCollider)
                {
                    // キャラが落ちる位置に別のオブジェクトが存在する場合、移動できないため失敗とする。（キャラはスタックしないものと考える）
                    Debug.LogError($"{ammo} が落ちるスペースがありません。");
                    return true;
                }
            }
            if (user.Location.Space.CollideAt(to, false, ammoMovement.HasTileCollider))
            {
                // 弾の落ちる位置が壁だった場合、移動できないため失敗とする。
                Debug.LogError($"{ammo} を壁の中に落とすことができません。");
                return true;
            }
            return AttackUtility.AssertTarget(hitObj, arg);
        }

        /// <summary>
        /// 攻撃が命中する位置に指定の対象（<see cref="RogueMethodArgument.TargetObj"/>）がいるか調べる。
        /// </summary>
        public static bool RequireTarget(IProjectileRogueMethodRange range, RogueObj self, in RogueMethodArgument arg, out RogueObj target)
        {
            var movement = MovementCalculator.Get(self);
            var direction = RogueMethodUtility.GetTargetDirection(self, arg);
            self.Main.Stats.Direction = direction;
            range.Raycast(self.Location, self.Position, direction, true, movement.HasTileCollider, out target, out _, out _);
            return AttackUtility.AssertTarget(target, arg);
        }

        public static bool RequireTarget(
            IRoguePredicator predicator, IRogueMethodRange range, RogueObj self, in RogueMethodArgument arg, out Vector2Int position)
        {
            var direction = RogueMethodUtility.GetTargetDirection(self, arg);
            self.Main.Stats.Direction = direction;
            if (!arg.TryGetTargetPosition(out position)) { position = self.Position + direction.Forward; }
            range.Predicate(predicator, self, 0f, null, position);
            if (predicator.Positions.Count == 0) return true;

            if (arg.TargetObj == null) return false;

            var targets = predicator.GetObjs(position);
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == arg.TargetObj) return false;
            }
            return true;
        }
    }
}
