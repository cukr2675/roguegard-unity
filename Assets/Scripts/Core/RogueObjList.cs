using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class RogueObjList //: IReadOnlyList<RogueObj>
    {
        private readonly List<RogueObj> objs;

        public RogueObj this[int index]
        {
            get => objs[index];
            set => objs[index] = value;
        }

        public int Count => objs.Count;

        [Objforming.CreateInstance]
        private RogueObjList(bool dummy) { }

        public RogueObjList()
        {
            objs = new List<RogueObj>();
        }

        public RogueObjList(RogueObjList other)
        {
            objs = new List<RogueObj>();
            for (int i = 0; i < other.Count; i++)
            {
                objs.Add(other[i]);
            }
        }

        public void Add(RogueObj item)
        {
            objs.Add(item);
        }

        /// <summary>
        /// 重複がないように追加する。
        /// </summary>
        /// <exception cref="System.ArgumentNullException"><paramref name="item"/> は null にできない。</exception>
        public bool TryAddUnique(RogueObj item)
        {
            if (item == null) throw new System.ArgumentNullException(nameof(item));
            if (objs.Contains(item)) return false;

            Add(item);
            return true;
        }

        public void Insert(int index, RogueObj item)
        {
            objs.Insert(index, item);
        }

        public bool Remove(RogueObj item)
        {
            return objs.Remove(item);
        }

        public void RemoveAt(int index)
        {
            objs.RemoveAt(index);
        }

        public void Clear()
        {
            objs.Clear();
        }

        public bool Contains(RogueObj item)
        {
            return objs.Contains(item);
        }

        public int IndexOf(RogueObj item)
        {
            return objs.IndexOf(item);
        }

        public void Sort(IComparer<RogueObj> comparer)
        {
            objs.Sort(comparer);
        }

        public void Sort(Spanning<RogueObj> sorted)
        {
            if (sorted.Count != Count) throw new System.ArgumentException("要素数が一致しません。");

            // 例外を投げるときソートをキャンセルしたいので、一通り例外判定してからソートする
            for (int i = 0; i < sorted.Count; i++)
            {
                if (!sorted.Contains(objs[i])) throw new System.ArgumentException($"{nameof(sorted)} が要素を網羅していません。");
            }

            objs.Clear();
            for (int i = 0; i < sorted.Count; i++)
            {
                objs.Add(sorted[i]);
            }
        }

        public bool Stack(RogueObj item, Vector2Int position, int maxStack)
        {
            var result = false;
            foreach (var obj in objs)
            {
                if (obj == null) continue; // null にスタックすることはできない。
                if (obj == item) continue; // 同一インスタンスにスタックすることはできない。
                if (obj.Position != position) continue;
                if (!obj.CanStack(item)) continue; // スタックできないオブジェクトにスタックすることはできない。

                // 二つのオブジェクトを重ねてひとつにする。
                var beforeStack = obj.Stack;
                var afterStack = Mathf.Min(obj.Stack + item.Stack, maxStack);
                if (!obj.TrySetStack(afterStack)) continue;
                item.TrySetStack(item.Stack - (afterStack - beforeStack));

                // スタックがゼロになったらそこで終了。
                if (item.Stack == 0) return true;

                // スタックがゼロにならなかったら他のスタック先を探す。
                result = true;
            }
            return result;
        }

        private IEnumerator<RogueObj> GetEnumerator() => objs.GetEnumerator();

        public override string ToString()
        {
            if (Count == 1)
            {
                return $"{this[0]} in {nameof(RogueObjList)}";
            }
            else
            {
                return $"{Count} items in {nameof(RogueObjList)}";
            }
        }

        public static implicit operator Spanning<RogueObj>(RogueObjList list) => list.objs;
    }
}
