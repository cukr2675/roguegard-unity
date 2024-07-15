using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;
using Roguegard.Extensions;

namespace Roguegard
{
    public class CommonWalk : ReferableScript, IActiveRogueMethod
    {
        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (self.Location?.Space.Tilemap == null) return false;

            var direction = RogueMethodUtility.GetTargetDirection(self, arg);
            var weightCalculator = WeightCalculator.Get(self);
            var loadCapacity = StatsEffectedValues.GetLoadCapacity(self);
            if (weightCalculator.SpaceWeight > loadCapacity)
            {
                if (RogueDevice.Primary.Player == self)
                {
                    RogueDevice.Add(DeviceKw.AppendText, "重くて動けない\n");
                }
                return false;
            }

            var deltaPosition = direction.Forward;
            var movement = MovementCalculator.Get(self);
            var asTile = movement.AsTile;
            var collide = movement.HasCollider;
            var tileCollide = movement.HasTileCollider;
            var sightCollide = movement.HasSightCollider;

            var position = self.Position;
            var target = position + deltaPosition;
            var userMovement = MovementCalculator.Get(self);
            var targetTile = self.Location.Space.Tilemap.GetTop(target);
            if (targetTile != null &&
                targetTile.Info.Category == CategoryKw.Pool &&
                !userMovement.SubIs(StdKw.PoolMovement))
            {
                // 水路には立ち入らない
                return false;
            }

            MessageWorkListener.TryOpenHandler(self.Location, position, out var positionHandler);
            MessageWorkListener.TryOpenHandler(self.Location, target, out var targetHandler);
            var unionHandler = MessageWorkListener.UnionHandlers(positionHandler, targetHandler);
            if (deltaPosition.x != 0 && deltaPosition.y != 0)
            {
                // 斜め移動
                // 壁以外は斜め移動可
                self.Main.Stats.Direction = RogueDirection.FromSignOrLowerLeft(target - position);
                var space = self.Location.Space;
                if (!space.CollideAt(new Vector2Int(position.x, target.y), false, tileCollide) &&
                    !space.CollideAt(new Vector2Int(target.x, position.y), false, tileCollide) &&
                    self.TryLocate(target, asTile, collide, tileCollide, sightCollide))
                {
                    unionHandler?.EnqueueWork(CreateWork(self, target, KeywordSpriteMotion.Walk, true));

                    // 移動に成功したら移動先のタイルを踏む
                    this.StepOn(self, activationDepth);
                    positionHandler?.Dispose();
                    targetHandler?.Dispose();
                    return true;
                }

                // 失敗しても方向転換する
                MainCharacterWorkUtility.TryAddTurn(self);
                positionHandler?.Dispose();
                targetHandler?.Dispose();
                return false;
            }
            else
            {
                // 縦移動・横移動
                self.Main.Stats.Direction = RogueDirection.FromSignOrLowerLeft(target - position);
                if (self.TryLocate(target, asTile, collide, tileCollide, sightCollide))
                {
                    unionHandler?.EnqueueWork(CreateWork(self, target, KeywordSpriteMotion.Walk, true));

                    // 移動に成功したら移動先のタイルを踏む
                    this.StepOn(self, activationDepth);
                    positionHandler?.Dispose();
                    targetHandler?.Dispose();
                    return true;
                }

                // 失敗しても方向転換する
                MainCharacterWorkUtility.TryAddTurn(self);
                positionHandler?.Dispose();
                targetHandler?.Dispose();
                return false;
            }
        }

        private static RogueCharacterWork CreateWork(RogueObj obj, Vector2Int position, ISpriteMotion spriteMotion, bool continues)
        {
            return RogueCharacterWork.CreateWalk(obj, position, obj.Main.Stats.Direction, spriteMotion, continues);
        }
    }
}
