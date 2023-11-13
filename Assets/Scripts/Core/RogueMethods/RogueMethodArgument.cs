using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// <see cref="IRogueMethod"/> の引数をまとめた構造体
    /// </summary>
    public readonly struct RogueMethodArgument
    {
        public static RogueMethodArgument Identity { get; } = default;

        public RogueObj TargetObj { get; }
        private readonly Vector2Int targetPosition;
        private readonly bool positioned;
        public int Count { get; }
        public Vector2 Vector { get; }
        public AffectableValue RefValue { get; }
        public RogueObj Tool { get; }
        public object Other { get; }

        public RogueMethodArgument(
            RogueObj targetObj = null,
            int count = default,
            Vector2 vector = default,
            AffectableValue value = null,
            RogueObj tool = null,
            object other = null)
        {
            targetPosition = Vector2Int.zero;
            positioned = false;

            TargetObj = targetObj;
            Count = count;
            Vector = vector;
            RefValue = value;
            Tool = tool;
            Other = other;
        }

        public RogueMethodArgument(
            Vector2Int targetPosition,
            RogueObj targetObj = null,
            int count = default,
            Vector2 vector = default,
            AffectableValue value = null,
            RogueObj tool = null,
            object other = null)
        {
            this.targetPosition = targetPosition;
            positioned = true;

            TargetObj = targetObj;
            Count = count;
            Vector = vector;
            RefValue = value;
            Tool = tool;
            Other = other;
        }

        public bool TryGetTargetPosition(out Vector2Int targetPosition)
        {
            if (positioned)
            {
                targetPosition = this.targetPosition;
                return true;
            }
            else
            {
                targetPosition =Vector2Int.zero;
                return false;
            }
        }
    }
}
