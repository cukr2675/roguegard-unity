using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class CommonWalk : ReferableScript, IActiveRogueMethod
    {
        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
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
            if (self.Location.Space.Tilemap.GetTop(target).Info.Category == CategoryKw.Pool &&
                !userMovement.SubIs(StdKw.PoolMovement))
            {
                // 水路には立ち入らない
                return false;
            }

            var trueVisible = MainCharacterWorkUtility.VisibleAt(self.Location, position) || MainCharacterWorkUtility.VisibleAt(self.Location, target);
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
                    if (trueVisible)
                    {
                        var item = CreateWork(self, target, KeywordBoneMotion.Walk, true);
                        RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
                    }

                    // 移動に成功したら移動先のタイルを踏む
                    this.StepOn(self, activationDepth);
                    return true;
                }

                // 失敗しても方向転換する
                MainCharacterWorkUtility.TryAddTurn(self);
                return false;
            }
            else
            {
                // 縦移動・横移動
                self.Main.Stats.Direction = RogueDirection.FromSignOrLowerLeft(target - position);
                if (self.TryLocate(target, asTile, collide, tileCollide, sightCollide))
                {
                    if (trueVisible)
                    {
                        var item = CreateWork(self, target, KeywordBoneMotion.Walk, true);
                        RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
                    }

                    // 移動に成功したら移動先のタイルを踏む
                    this.StepOn(self, activationDepth);
                    return true;
                }

                // 失敗しても方向転換する
                MainCharacterWorkUtility.TryAddTurn(self);
                return false;
            }
        }

        private static RogueCharacterWork CreateWork(RogueObj obj, Vector2Int position, IBoneMotion boneMotion, bool continues)
        {
            return RogueCharacterWork.CreateWalk(obj, position, obj.Main.Stats.Direction, boneMotion, continues);
        }
    }
}
