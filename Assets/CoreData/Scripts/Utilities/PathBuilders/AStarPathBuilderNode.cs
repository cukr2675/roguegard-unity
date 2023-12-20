using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    internal class AStarPathBuilderNode
    {
        /// <summary>
        /// true のとき、このノードの上には移動できない
        /// </summary>
        public bool HasCollider { get; set; }

        /// <summary>
        /// true のとき、このノードの隣を斜め移動で通過できない
        /// </summary>
        public bool HasCornerCollider { get; set; }

        public int G { get; private set; }

        public int H { get; private set; }

        public int F => G + H;

        public Vector2Int CameFrom { get; private set; }

        private State state;

        public bool IsOpen => state == State.Open;

        public void Reset()
        {
            G = int.MaxValue;
            H = 0;
            state = State.Unopened;
        }

        public bool TryOpen(int costedG, Vector2Int relativePosition, Vector2Int from)
        {
            var h = Mathf.Abs(relativePosition.x) + Mathf.Abs(relativePosition.y);

            if (costedG + h < F)
            {
                G = costedG;
                H = h;
                CameFrom = from;
                state = State.Open;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Close()
        {
            state = State.Closed;
        }

        private enum State
        {
            Unopened,
            Open,
            Closed
        }
    }
}
