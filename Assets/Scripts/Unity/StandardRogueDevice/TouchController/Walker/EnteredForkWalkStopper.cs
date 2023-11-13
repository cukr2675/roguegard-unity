using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    public class EnteredForkWalkStopper : IStatedWalkStopper
    {
        public bool GetStop(RogueObj self)
        {
            var view = self.Get<ViewInfo>();
            var position = self.Position;
            var direction = self.Main.Stats.Direction;
            var forward = direction.Forward;
            var forwardHasCollider = view.HasStopperAt(position + forward);
            //var forward2HasCollider = view.HasStopperAt(position + forward * 2); // 2マス先が床なら部屋の可能性あり

            var leftForward = direction.Rotate(1).Forward;
            var rightForward = direction.Rotate(-1).Forward;
            //var leftForwardHasCollider = view.HasStopperAt(position + leftForward);
            //var rightForwardHasCollider = view.HasStopperAt(position + rightForward);
            //var leftForward2HasCollider = view.HasStopperAt(position + leftForward + forward);
            //var rightForward2HasCollider = view.HasStopperAt(position + rightForward + forward);
            var leftBackHasCollider = view.HasStopperAt(position - rightForward);
            var rightBackHasCollider = view.HasStopperAt(position - leftForward);

            var left = direction.Rotate(2).Forward;
            var right = direction.Rotate(-2).Forward;
            var leftHasCollider = view.HasStopperAt(position + left);
            var rightHasCollider = view.HasStopperAt(position + right);

            // そのまま進む
            // ？？？？？　　　　？？？？？　　　　？■　■？　　　　？　　■？　　　　？■■　■
            // ？■↑■？　　　　？　↑　？　　　　？■↑　？　　　　？■↑　？　　　　？■↑　■
            // ？■＠■？　　　　？■＠■？　　　　？■＠■？　　　　？■＠■？　　　　？■＠■■
            // ？■　■？　　　　？■　■？　　　　？■　■？　　　　？■　■？　　　　？■　■？
            // ？■　■？　　　　？■　■？　　　　？■　■？　　　　？■　■？　　　　？■　■？

            // 分岐路・暗い部屋で止まる
            // ？？？？？　　　　？？？？？　　　　？？？？？　　　　？？？？？　　　　？？？？？　　　　？？？？？　　　　？？？？？
            // ？■↑■？　　　　？■↑　？　　　　？　↑■？　　　　？　↑　？　　　　？■↑■？　　　　？■↑　？　　　　？　↑　？
            // ？■＠　？　　　　？■＠　？　　　　？■＠　？　　　　？■＠　？　　　　？　＠　？　　　　？　＠　？　　　　？　＠　？
            // ？■　■？　　　　？■　■？　　　　？■　■？　　　　？■　■？　　　　？■　■？　　　　？■　■？　　　　？■　■？
            // ？■　■？　　　　？■　■？　　　　？■　■？　　　　？■　■？　　　　？■　■？　　　　？■　■？　　　　？■　■？
            var enteredFork =
                !forwardHasCollider && leftBackHasCollider && rightBackHasCollider &&
                (!leftHasCollider || !rightHasCollider);

            // 部屋の出口で止まる
            // ■■■■？　　　　■■■■？
            // ■　↑■？　　　　■　↑　？
            // ■　＠　？　　　　■　＠　？
            // ■　　■？　　　　■　　■？
            // ■■■■？　　　　■■■■？
            var exitRoom =
                !forwardHasCollider && !leftHasCollider && !rightHasCollider &&
                (leftBackHasCollider || rightBackHasCollider);

            return enteredFork || exitRoom; //|| enteredRoom;




            //// 部屋の入口で止まる
            //// ■■■■■　　　　？■■■■
            //// ■　　　■　　　　？■　　■
            //// ■　↑　■　　　　？■↑　■
            //// ■■＠■■　　　　？■＠■■
            //// ？■　■？　　　　？■　■？
            //// ？■　■？　　　　？■　■？
            //var enteredRoom =
            //    !forwardHasCollider && !forward2HasCollider && leftHasCollider && rightHasCollider &&
            //    ((!leftForwardHasCollider && !leftForward2HasCollider) || (!rightForwardHasCollider && !rightForward2HasCollider));

            //// 部屋の入口（と思われる場所）で止まる
            //// ？？？？？　　　　？？？？？　　　　？　　　？　　　　？■　　？
            //// ？　↑　？　　　　？■↑　？　　　　？　↑　？　　　　？■↑　？
            //// ？■＠■？　　　　？■＠■？　　　　？■＠■？　　　　？■＠■？
            //// ？■　■？　　　　？■　■？　　　　？■　■？　　　　？■　■？
            //// ？■　■？　　　　？■　■？　　　　？■　■？　　　　？■　■？
            //forward2HasCollider = view.HasStopperAt(position + forward * 2, false);
            //leftForward2HasCollider = view.HasStopperAt(position + leftForward + forward, false);
            //rightForward2HasCollider = view.HasStopperAt(position + rightForward + forward, false);
            //var enteredRoom =
            //    !forwardHasCollider && !forward2HasCollider && leftHasCollider && rightHasCollider &&
            //    ((!leftForwardHasCollider && !leftForward2HasCollider) || (!rightForwardHasCollider && !rightForward2HasCollider));
        }
    }
}
