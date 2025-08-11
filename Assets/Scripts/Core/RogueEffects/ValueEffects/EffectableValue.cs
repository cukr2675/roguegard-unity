using System.Collections.Generic;

namespace Roguegard
{
    public class EffectableValue : System.IDisposable
    {
        public float BaseMainValue { get; set; }
        public float MainValue { get; set; }
        public KeywordValueMap SubValues { get; } = new();

        private static readonly Stack<EffectableValue> poolingStack = new();

        private EffectableValue() { }

        public static EffectableValue Get()
        {
            if (poolingStack.Count >= 1)
            {
                return poolingStack.Pop();
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
            poolingStack.Push(this);
        }
    }
}
