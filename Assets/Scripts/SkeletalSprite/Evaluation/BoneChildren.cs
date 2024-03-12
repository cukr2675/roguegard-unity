using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    internal class BoneChildren<T> : ISortableBoneChildren<T>//, IReadOnlyList<T>
        where T : ISortableBone<T>
    {
        /// <summary>
        /// <see cref="ISortableBone.NormalOrderInParent"/> で並べ替えた子のリスト。
        /// </summary>
        private readonly List<T> _normalFrontChildren;

        /// <summary>
        /// <see cref="ISortableBone.NormalOrderInParent"/> で並べ替えた子のリスト。
        /// </summary>
        private readonly List<T> _normalRearChildren;

        /// <summary>
        /// <see cref="ISortableBone.BackOrderInParent"/> で並べ替えた子のリスト。
        /// </summary>
        private readonly List<T> _backFrontChildren;

        /// <summary>
        /// <see cref="ISortableBone.BackOrderInParent"/> で並べ替えた子のリスト。
        /// </summary>
        private readonly List<T> _backRearChildren;

        IReadOnlyList<T> ISortableBoneChildren<T>.NormalFrontChildren => _normalFrontChildren;
        IReadOnlyList<T> ISortableBoneChildren<T>.NormalRearChildren => _normalRearChildren;
        IReadOnlyList<T> ISortableBoneChildren<T>.BackFrontChildren => _backFrontChildren;
        IReadOnlyList<T> ISortableBoneChildren<T>.BackRearChildren => _backRearChildren;

        public T this[int index]
        {
            get
            {
                if (index < _normalFrontChildren.Count)
                {
                    return _normalFrontChildren[index];
                }
                else
                {
                    return _normalRearChildren[index - _normalFrontChildren.Count];
                }
            }
        }

        public int Count { get; private set; }

        public BoneChildren()
        {
            _normalFrontChildren = new List<T>();
            _normalRearChildren = new List<T>();
            _backFrontChildren = new List<T>();
            _backRearChildren = new List<T>();
            Count = 0;
        }

        public void AddChild(T bone)
        {
            AddChild(bone, false);
            AddChild(bone, true);
            Count++;

            void AddChild(T bone, bool back)
            {
                List<T> frontChildren, rearChildren;
                if (back)
                {
                    frontChildren = _backFrontChildren;
                    rearChildren = _backRearChildren;
                }
                else
                {
                    frontChildren = _normalFrontChildren;
                    rearChildren = _normalRearChildren;
                }
                var orderInParent = GetOrderInParent(bone);

                // orderInParent が同じときはその手前に追加する。
                // SortingGroup はヒエラルキーで下にある（＝インデックスが大きい）ほど手前に表示される
                //  -> BoneChildrenSorter では最初に裏地を Front から Rear へ向かって設定する
                // そのため、インデックスが小さいほど手前とする。
                if (orderInParent <= 0f)
                {
                    for (int i = 0; i < frontChildren.Count; i++)
                    {
                        var child = frontChildren[i];
                        if (GetOrderInParent(child) >= orderInParent)
                        {
                            frontChildren.Insert(i, bone);
                            return;
                        }
                    }
                    frontChildren.Add(bone);
                }
                else
                {
                    for (int i = 0; i < rearChildren.Count; i++)
                    {
                        var child = rearChildren[i];
                        if (GetOrderInParent(child) >= orderInParent)
                        {
                            rearChildren.Insert(i, bone);
                            return;
                        }
                    }
                    rearChildren.Add(bone);
                }

                float GetOrderInParent(T bone)
                {
                    if (back) return bone.BackOrderInParent;
                    else return bone.NormalOrderInParent;
                }
            }
        }

        private IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _normalFrontChildren.Count; i++)
            {
                yield return _normalFrontChildren[i];
            }
            for (int i = 0; i < _normalRearChildren.Count; i++)
            {
                yield return _normalRearChildren[i];
            }
        }
    }
}
