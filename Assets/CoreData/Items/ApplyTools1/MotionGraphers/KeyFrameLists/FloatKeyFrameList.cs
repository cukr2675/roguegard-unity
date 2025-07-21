using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class FloatKeyFrameList
    {
        private readonly List<KeyFrame> items;

        public KeyFrame this[int index] => items[index];

        public int Count => items.Count;

        public FloatKeyFrameList()
        {
            items = new List<KeyFrame>();
        }

        [Objforming.CreateInstance, SuppressMessage("Style", "IDE0051")]
        private FloatKeyFrameList(bool _) { }

        public bool TryGetValue(float time, out float value)
        {
            var index = IndexOf(time);
            if (index >= 0)
            {
                value = items[index].Value;
                return true;
            }
            else
            {
                value = 0f;
                return false;
            }
        }

        public float GetLerp(float time)
        {
            if (items.Count == 0) return 0f;

            for (int i = 1; i < Count; i++)
            {
                if (items[i].Time <= time) continue;

                var rate = Mathf.InverseLerp(items[i - 1].Time, items[i].Time, time);
                return Mathf.Lerp(items[i - 1].Value, items[i].Value, rate);
            }
            return items[^1].Value;
        }

        private int IndexOf(float time)
        {
            for (int i = 0; i < Count; i++)
            {
                if (items[i].Time == time) return i;
            }
            return -1;
        }

        public void Set(float time, float value)
        {
            var index = IndexOf(time);
            if (index >= 0)
            {
                items[index] = new KeyFrame(value, time);
            }
            else
            {
                // Time の昇順になるよう追加する
                for (int i = 0; i < Count; i++)
                {
                    if (items[i].Time < time) continue;

                    items.Insert(i, new KeyFrame(value, time));
                    return;
                }
                items.Add(new KeyFrame(value, time));
            }
        }

        public void Remove(float time)
        {
            var index = IndexOf(time);
            if (index >= 0)
            {
                items.RemoveAt(index);
            }
        }

        public IEnumerable<float> ToKeyTimes() => items.Select(x => x.Time);

        [Objforming.Formable]
        public struct KeyFrame
        {
            public float Value { get; set; }
            public float Time { get; set; }

            public KeyFrame(float value, float time)
            {
                Value = value;
                Time = time;
            }
        }
    }
}
