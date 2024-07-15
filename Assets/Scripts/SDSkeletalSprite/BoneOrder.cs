using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace SDSSprite
{
    /// <summary>
    /// このインターフェースを実装するクラスは不変にする。（等価判定でキャッシュするため）
    /// </summary>
    public class BoneOrder
    {
        /// <summary>
        /// 適用するボーンを子も含めて前後反転する。
        /// </summary>
        public IReadOnlyList<BoneBack> LocalBacks => _localBacks;
        private readonly BoneBack[] _localBacks;

        /// <summary>
        /// スプライトの表示最適化によって動作が若干変わる可能性あり？
        /// </summary>
        public IReadOnlyList<BoneReorder> Reorders => _reorders;
        private readonly BoneReorder[] _reorders;

        public BoneOrder(IEnumerable<BoneBack> localBacks, IEnumerable<BoneReorder> reorders)
        {
            _localBacks = localBacks.ToArray();
            _reorders = reorders.ToArray();
        }

        public static bool Equals(BoneOrder left, BoneOrder right)
        {
            if (left == right) return true;
            if (left == null ^ right == null) return false;
            if (left.LocalBacks.Count != right.LocalBacks.Count) return false;
            if (left.Reorders.Count != right.Reorders.Count) return false;

            for (int i = 0; i < left.LocalBacks.Count; i++)
            {
                if (!left.LocalBacks[i].Equals(right.LocalBacks[i])) return false;
            }
            for (int i = 0; i < left.Reorders.Count; i++)
            {
                if (!left.Reorders[i].Equals(right.Reorders[i])) return false;
            }
            return true;
        }
    }
}
