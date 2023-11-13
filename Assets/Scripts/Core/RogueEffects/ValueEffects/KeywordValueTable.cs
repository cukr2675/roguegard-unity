using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// 未登録の値は 0 に相当する連想配列クラス。
    /// </summary>
    public class KeywordValueTable
    {
        private readonly Dictionary<IKeyword, float> subValues = new Dictionary<IKeyword, float>();

        public float this[IKeyword key]
        {
            get => subValues.TryGetValue(key, out var value) ? value : 0f;
            set => subValues[key] = value;
        }

        public bool Remove(IKeyword key)
        {
            return subValues.Remove(key);
        }

        public void Clear()
        {
            subValues.Clear();
        }

        /// <summary>
        /// 指定のキーの値が非ゼロであれば true を取得する。
        /// </summary>
        public bool Is(IKeyword key)
        {
            return this[key] != 0f;
        }

        public void CopyTo(KeywordValueTable table)
        {
            table.Clear();
            foreach (var pair in subValues)
            {
                table.subValues.Add(pair.Key, pair.Value);
            }
        }
    }
}
