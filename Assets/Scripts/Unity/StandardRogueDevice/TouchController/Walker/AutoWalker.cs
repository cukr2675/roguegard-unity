using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    internal static class AutoWalker
    {
        public static bool AutoWalking(RogueObj self, out Vector2Int targetPosition, WalkStopper walkStopper, bool force)
        {
            walkStopper.UpdateStatedStop();

            var view = ViewInfo.Get(self);
            var position = self.Position;
            var direction = self.Main.Stats.Direction;
            var forward = direction.Forward;
            var forwardHasCollider = view.HasStopperAt(position + forward);

            var left = direction.Rotate(2).Forward;
            var right = direction.Rotate(-2).Forward;
            var leftHasCollider = view.HasStopperAt(position + left);
            var rightHasCollider = view.HasStopperAt(position + right);

            var leftBack = direction.Rotate(3).Forward;
            var rightBack = direction.Rotate(-3).Forward;
            var leftBackHasCollider = view.HasStopperAt(position + leftBack); // 左に曲がるとき、左斜め後ろが壁なら通路と考える。
            var rightBackHasCollider = view.HasStopperAt(position + rightBack); // 右に曲がるとき、右斜め後ろが壁なら通路と考える。



            // 前
            if (!forwardHasCollider)
            {
                // 前方が壁でなければ前進する。
                targetPosition = position + forward;
                walkStopper.UpdatePositionedStop(targetPosition);
                return (!walkStopper.StatedStop && !walkStopper.PositionedStop) || force;
            }

            // 左右のどちらか片方のみ壁でないかつ通路である場合、壁でないほうに進む。
            if (!leftHasCollider && rightHasCollider && leftBackHasCollider)
            {
                targetPosition = position + left;
                walkStopper.UpdatePositionedStop(targetPosition);
                return (!walkStopper.StatedStop && !walkStopper.PositionedStop) || force;
            }
            if (!rightHasCollider && leftHasCollider && rightBackHasCollider)
            {
                targetPosition = position + right;
                walkStopper.UpdatePositionedStop(targetPosition);
                return (!walkStopper.StatedStop && !walkStopper.PositionedStop) || force;
            }

            targetPosition = self.Position;
            return false;
        }
    }
}
