using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    public class WalkStopper
    {
        private readonly RogueObj obj;

        private readonly List<StatedItem> statedItems;

        private readonly List<PositionedItem> positionedItems;

        public bool StatedStop { get; private set; }

        public bool PositionedStop { get; private set; }

        public WalkStopper(RogueObj self)
        {
            obj = self;
            statedItems = new List<StatedItem>();
            positionedItems = new List<PositionedItem>();
        }

        public void AddStopper(IStatedWalkStopper walkStopper, bool stopsContinuously = false)
        {
            var item = new StatedItem(walkStopper, stopsContinuously);
            statedItems.Add(item);
        }

        public void AddStopper(IPositionedWalkStopper walkStopper, bool stopsContinuously = false)
        {
            var item = new PositionedItem(walkStopper, stopsContinuously);
            positionedItems.Add(item);
        }

        public void Initialize()
        {
            // lastStop を初期化する。
            UpdateStatedStop();
            UpdatePositionedStop(obj.Position);
        }

        public void UpdateStatedStop()
        {
            var result = false;
            foreach (var item in statedItems)
            {
                // item の lastStop を更新するため、全ての item を網羅する必要がある。
                result |= item.GetStop(obj);
            }
            StatedStop = result;
        }

        public void UpdatePositionedStop(Vector2Int targetPosition)
        {
            var result = false;
            foreach (var item in positionedItems)
            {
                // item の lastStop を更新するため、全ての item を網羅する必要がある。
                result |= item.GetStop(obj, targetPosition);
            }
            PositionedStop = result;
        }

        private class StatedItem
        {
            /// <summary>
            /// 連続で停止判定が出たときも停止するかどうか。
            /// </summary>
            public bool StopsContinuously { get; set; }

            private bool lastStop;

            private readonly IStatedWalkStopper walkStopper;

            public StatedItem(IStatedWalkStopper walkStopper, bool stopsContinuously)
            {
                this.walkStopper = walkStopper;
                StopsContinuously = stopsContinuously;
            }

            public bool GetStop(RogueObj self)
            {
                var stop = walkStopper.GetStop(self);
                var result = stop;
                if (!StopsContinuously && lastStop)
                {
                    result = false;
                }
                lastStop = stop;
                return result;
            }
        }

        private class PositionedItem
        {
            /// <summary>
            /// 連続で停止判定が出たときも停止するかどうか。
            /// </summary>
            public bool StopsContinuously { get; set; }

            private bool lastStop;

            private readonly IPositionedWalkStopper walkStopper;

            public PositionedItem(IPositionedWalkStopper walkStopper, bool stopsContinuously)
            {
                this.walkStopper = walkStopper;
                StopsContinuously = stopsContinuously;
            }

            public bool GetStop(RogueObj self, Vector2Int targetPosition)
            {
                var stop = walkStopper.GetStop(self, targetPosition);
                var result = stop;
                if (!StopsContinuously && lastStop)
                {
                    result = false;
                }
                lastStop = stop;
                return result;
            }
        }
    }
}
