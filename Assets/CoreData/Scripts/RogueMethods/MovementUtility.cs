using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public static class MovementUtility
    {
        public static bool TryGetApproachDirection(RogueObj self, Vector2Int targetPosition, bool walkAvoidTileMovement, out RogueDirection direction)
        {
            if (RogueDirection.TryFromSign(targetPosition - self.Position, out direction))
            {
                var result = CheckWalk(self, direction, walkAvoidTileMovement);
                if (result) return true;

                // まっすぐ移動できなかった場合は、左右45度まで回り込む。
                if (!result)
                {
                    // ２方向のうち、目標位置に近いほうを先に試行する。
                    var directionA = direction.Rotate(-1);
                    var directionB = direction.Rotate(+1);
                    var sqrDistanceA = (self.Position + directionA.Forward - targetPosition).sqrMagnitude;
                    var sqrDistanceB = (self.Position + directionB.Forward - targetPosition).sqrMagnitude;
                    if (sqrDistanceA <= sqrDistanceB)
                    {
                        if (!result)
                        {
                            direction = directionA;
                            result = CheckWalk(self, direction, walkAvoidTileMovement);
                            if (result) return true;
                        }
                        if (!result)
                        {
                            direction = directionB;
                            result = CheckWalk(self, direction, walkAvoidTileMovement);
                            if (result) return true;
                        }
                    }
                    else
                    {
                        if (!result)
                        {
                            direction = directionB;
                            result = CheckWalk(self, direction, walkAvoidTileMovement);
                            if (result) return true;
                        }
                        if (!result)
                        {
                            direction = directionA;
                            result = CheckWalk(self, direction, walkAvoidTileMovement);
                            if (result) return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool CheckWalk(RogueObj self, RogueDirection direction, bool checkTileCategory)
        {
            var deltaPosition = direction.Forward;
            var movement = MovementCalculator.Get(self);
            var collide = movement.HasCollider;
            var tileCollide = movement.HasTileCollider;

            var space = self.Location.Space;
            var position = self.Position;
            var target = position + deltaPosition;
            if (deltaPosition.x != 0 && deltaPosition.y != 0)
            {
                // 斜め移動
                // 壁以外は斜め移動可
                if (!space.CollideAt(new Vector2Int(position.x, target.y), false, tileCollide) &&
                    !space.CollideAt(new Vector2Int(target.x, position.y), false, tileCollide) &&
                    !space.CollideAt(target, collide, tileCollide))
                {
                    if (checkTileCategory)
                    {
                        // 水地形に入らないようにする
                        var tile = space.Tilemap.GetTop(target);
                        if (tile.Info.Category == CategoryKw.Pool && !movement.SubIs(StdKw.PoolMovement)) return false;
                    }

                    return true;
                }
                return false;
            }
            else
            {
                // 縦移動・横移動
                if (!space.CollideAt(target, collide, tileCollide))
                {
                    if (checkTileCategory)
                    {
                        // 水地形に入らないようにする
                        var tile = space.Tilemap.GetTop(target);
                        if (tile.Info.Category == CategoryKw.Pool && !movement.SubIs(StdKw.PoolMovement)) return false;
                    }

                    return true;
                }
                return false;
            }
        }
    }
}
