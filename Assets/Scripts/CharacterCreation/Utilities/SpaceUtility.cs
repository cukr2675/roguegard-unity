using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class SpaceUtility
    {
        public static bool TryLocate(RogueObj self, RogueObj location, Vector2Int position, StackOption stackOption = StackOption.Default)
        {
            var movement = MovementCalculator.Get(self);
            return self.TryLocate(location, position, movement.AsTile, movement.HasCollider, movement.HasTileCollider, movement.HasSightCollider, stackOption);
        }

        public static bool TryLocate(RogueObj self, Vector2Int position, StackOption stackOption = StackOption.Default)
        {
            return TryLocate(self, self.Location, position, stackOption);
        }

        public static bool TryLocate(RogueObj self, RogueObj location, StackOption stackOption = StackOption.Default)
        {
            if (location != null && location.Space.Tilemap != null)
                throw new System.ArgumentException("タイルマップを持つオブジェクトへ移動する場合、位置（Position）が必要です。");

            return TryLocate(self, location, Vector2Int.zero, stackOption);
        }

        public static void Restack(RogueObj obj, StackOption stackOption = StackOption.Default)
        {
            var maxStack = obj.GetMaxStack(stackOption);
            obj.Location.Space.Stack(obj, obj.Position, maxStack);
        }

        /// <summary>
        /// <see cref="RogueObj.TrySetStack(int, RogueObj)"/> に失敗したときは代わりに <paramref name="stack"/> - 1 個のクローンを生成する。
        /// </summary>
        private static bool TryStackClone(RogueObj obj, int stack)
        {
            var stackResult = obj.TrySetStack(stack);
            if (!stackResult)
            {
                if (obj.Location == null) return false;

                for (int i = 0; i < stack - 1; i++)
                {
                    var clone = obj.Clone();
                    if (clone == null || !TryLocate(clone, obj.Location, obj.Position)) return false;
                }
            }
            return true;
        }

        public static RogueObj Divide(RogueObj obj, int dividedObjStack)
        {
            if (dividedObjStack <= 0 || obj.Stack < dividedObjStack) throw new System.ArgumentOutOfRangeException(nameof(dividedObjStack));

            if (dividedObjStack == obj.Stack) return obj;

            var clone = obj.Clone();
            clone.TrySetStack(dividedObjStack);
            obj.TrySetStack(obj.Stack - dividedObjStack);
            return clone;
        }

        public static bool TryDividedLocate(RogueObj obj, int dividedObjStack, out RogueObj dividedObj)
        {
            if (dividedObjStack <= 0 || obj.Stack < dividedObjStack) throw new System.ArgumentOutOfRangeException(nameof(dividedObjStack));

            if (dividedObjStack == obj.Stack)
            {
                dividedObj = obj;
                return true;
            }

            var clone = obj.Clone();
            clone.TrySetStack(dividedObjStack);

            // 分割による移動は LocateRogueMethod を実行しなくてよいものとする
            if (!TryLocate(clone, obj.Location, obj.Position, StackOption.NotStack))
            {
                dividedObj = default;
                return false;
            }

            obj.TrySetStack(obj.Stack - dividedObjStack);
            dividedObj = clone;
            return true;
        }

        public static bool ObjIsGlued(RogueObj obj)
        {
            var movement = MovementCalculator.Get(obj);
            return movement.SubIs(CharacterCreationKw.Glued);
        }

        public static void GetTop(RogueSpace space, Vector2Int position, out RogueObj tileObj, out IRogueTile tile)
        {
            tileObj = null;
            for (int i = 0; i < space.Objs.Count; i++)
            {
                var spaceObj = space.Objs[i];
                if (spaceObj == null || spaceObj.Position != position) continue;

                if (spaceObj.AsTile)
                {
                    // 最後に上書きしたオブジェクトをトップタイルとする。
                    tileObj = spaceObj;
                }
            }

            // タイルオブジェクトが存在する場合はオブジェクトを、存在しない場合はタイルを返す。
            if (tileObj != null)
            {
                tile = default;
            }
            else
            {
                tile = space.Tilemap.GetTop(position);
            }
        }

        /// <summary>
        /// <paramref name="space"/> 空間内で当たり判定を行う。オブジェクトかタイルに当たったとき true を返す。
        /// </summary>
        /// <param name="position">当たったオブジェクトかタイルの位置。何にも当たらなかったときは判定範囲の終点を返す。</param>
        /// <param name="dropPosition">オブジェクトかタイルに当たったアイテムが落下する位置。何にも当たらなかったときは判定範囲の終点に落下する。</param>
        public static bool Raycast(
            RogueObj space, Vector2Int origin, RogueDirection direction, int maxDistance, bool cutsCorners, bool collide, bool tileCollide,
            out RogueObj hitObj, out Vector2Int position, out Vector2Int dropPosition)
        {
            var locationSpace = space.Space;

            // 斜め移動しないときと壁に当たらないときは、角抜けの判定を省く。
            if (direction.Forward.x == 0 || direction.Forward.y == 0 || !tileCollide) { cutsCorners = true; }

            var currentPosition = origin;
            hitObj = null;
            for (int i = 0; i < maxDistance; i++)
            {
                var nextPosition = currentPosition + direction.Forward;
                if (!cutsCorners)
                {
                    var cornerPosition = currentPosition + direction.Rotate(-1).Forward;
                    if (locationSpace.CollideAt(cornerPosition, false, tileCollide))
                    {
                        // 角抜けしないとき角に当たったら止める。
                        position = cornerPosition;
                        dropPosition = currentPosition;
                        return true;
                    }
                    cornerPosition = currentPosition + direction.Rotate(1).Forward;
                    if (locationSpace.CollideAt(cornerPosition, false, tileCollide))
                    {
                        // 角抜けしないとき角に当たったら止める。
                        position = cornerPosition;
                        dropPosition = currentPosition;
                        return true;
                    }
                }
                if (locationSpace.CollideAt(nextPosition, collide, tileCollide))
                {
                    if (collide) { hitObj = locationSpace.GetColliderObj(nextPosition); }
                    position = nextPosition;
                    if (hitObj == null) { dropPosition = currentPosition; } // 壁に当たったとき、壁の手前に落とす。
                    else { dropPosition = nextPosition; } // オブジェクトに当たったとき、オブジェクトの真下に落とす。
                    return true;
                }
                currentPosition = nextPosition;
            }
            // 落下して地面に当たる
            position = currentPosition;
            dropPosition = currentPosition;
            return false;
        }
    }
}
