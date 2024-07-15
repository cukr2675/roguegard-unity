using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class EffectableValue : System.IDisposable
    {
        public float BaseMainValue { get; set; }
        public float MainValue { get; set; }
        public KeywordValueTable SubValues { get; } = new KeywordValueTable();

        private static readonly Stack<EffectableValue> stack = new Stack<EffectableValue>();

        private EffectableValue() { }

        public static EffectableValue Get()
        {
            if (stack.Count >= 1)
            {
                return stack.Pop();
            }
            else
            {
                return new EffectableValue();
            }
        }

        public void Initialize(float baseValue)
        {
            BaseMainValue = baseValue;
            MainValue = baseValue;
            SubValues.Clear();
        }

        public void CopyTo(EffectableValue value)
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
