using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class AffectableValue : System.IDisposable
    {
        public float BaseMainValue { get; set; }
        public float MainValue { get; set; }
        public KeywordValueTable SubValues { get; } = new KeywordValueTable();

        private static readonly Stack<AffectableValue> stack = new Stack<AffectableValue>();

        private AffectableValue() { }

        public static AffectableValue Get()
        {
            if (stack.Count >= 1)
            {
                return stack.Pop();
            }
            else
            {
                return new AffectableValue();
            }
        }

        public void Initialize(float baseValue)
        {
            BaseMainValue = baseValue;
            MainValue = baseValue;
            SubValues.Clear();
        }

        public void CopyTo(AffectableValue value)
        {
            value.BaseMainValue = BaseMainValue;
            value.MainValue = MainValue;
            SubValues.CopyTo(value.SubValues);
        }

        public void Dispose()
        {
            stack.Push(this);
        }
    }
}
