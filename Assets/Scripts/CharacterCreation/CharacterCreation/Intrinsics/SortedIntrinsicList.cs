using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.CharacterCreation
{
    public class SortedIntrinsicList : ISortedIntrinsicList
    {
        private readonly ISortedIntrinsic[] sortedIntrinsics;

        public SortedIntrinsicList(IEnumerable<IReadOnlyIntrinsic> intrinsics, ICharacterCreationData characterCreationData)
        {
            sortedIntrinsics = intrinsics.Select(x => x.Option.CreateSortedIntrinsic(x, characterCreationData)).ToArray();
        }

        public void Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base)
        {
            foreach (var intrinsic in sortedIntrinsics)
            {
                intrinsic.Open(self, infoSetType, polymorph2Base);
            }
        }

        public void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph)
        {
            foreach (var intrinsic in sortedIntrinsics)
            {
                intrinsic.Close(self, infoSetType, base2Polymorph);
            }
        }

        public void Reopen(RogueObj self, MainInfoSetType infoSetType, int deltaLv)
        {
            // すべてを閉じてから開きなおすとスキルの並びが変わってしまうため、
            // Reopen 前のレベルから現在のレベルまでの Intrinsic だけ更新する。
            // 同じ Intrinsic を複数回習得しないよう気を付ける。
            var selfLv = self.Main.Stats.Lv;
            if (deltaLv == +1)
            {
                foreach (var intrinsic in sortedIntrinsics)
                {
                    if (intrinsic.Lv == selfLv)
                    {
                        intrinsic.LevelUpToLv(self, infoSetType);
                    }
                }
            }
            else if (deltaLv == -1)
            {
                foreach (var intrinsic in sortedIntrinsics)
                {
                    if (intrinsic.Lv == selfLv + 1)
                    {
                        intrinsic.LevelDownFromLv(self, infoSetType);
                    }
                }
            }
        }
    }
}
