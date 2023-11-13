using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class RogueMethodArgumentBuilder
    {
        public RogueObj TargetObj { get; set; }
        private Vector2Int targetPosition;
        private bool positioned;
        public int Count { get; set; }
        public Vector2 Vector { get; set; }
        public AffectableValue RefValue { get; set; }
        public RogueObj Tool { get; set; }
        public object Other { get; set; }

        public void SetArgument(in RogueMethodArgument arg)
        {
            TargetObj = arg.TargetObj;
            positioned = arg.TryGetTargetPosition(out var targetPosition);
            this.targetPosition = targetPosition;
            Count = arg.Count;
            Vector = arg.Vector;
            RefValue = arg.RefValue;
            Tool = arg.Tool;
            Other = arg.Other;
        }

        public void SetTargetPosition(Vector2Int targetPosition)
        {
            this.targetPosition = targetPosition;
            positioned = true;
        }

        public void ClearTargetPosition()
        {
            positioned = false;
        }

        public RogueMethodArgument ToArgument()
        {
            if (positioned)
            {
                return new RogueMethodArgument(
                    targetPosition,
                    TargetObj,
                    Count,
                    Vector,
                    RefValue,
                    Tool,
                    Other);
            }
            else
            {
                return new RogueMethodArgument(
                    TargetObj,
                    Count,
                    Vector,
                    RefValue,
                    Tool,
                    Other);
            }
        }
    }
}
