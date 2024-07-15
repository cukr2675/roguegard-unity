using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class ShoutSkill : MPSkillIntrinsicOptionScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(parent, lv);
        }

        [Objforming.Formable]
        private class SortedIntrinsic : MPSkillSortedIntrinsic<SortedIntrinsic>
        {
            public override IRogueMethodTarget Target => null;
            public override IRogueMethodRange Range => null;
            public override int RequiredMP => 2;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                // 同じフロアにいる敵を隣に引き寄せる
                var random = RogueRandom.Primary;
                var location = self.Location;
                var locationObjs = location.Space.Objs;
                for (int i = 0; i < locationObjs.Count; i++)
                {
                    var obj = locationObjs[i];
                    if (obj == null || !StatsEffectedValues.AreVS(self, obj)) continue;

                    // 隣に引き寄せる
                    if (this.LocateNextToObj(obj, self, activationDepth, self)) continue;

                    // 隣に移動できなければ同じ部屋のランダム位置に移動させる
                    if (location.Space.TryGetRandomPositionInRoom(random, out var position))
                    {
                        this.Locate(obj, self, location, position, activationDepth);
                    }
                }

                return true;
            }

            public override int GetATK(RogueObj self, out bool additionalEffect)
            {
                // 同じフロアにいる敵を隣に引き寄せる
                additionalEffect = true;
                return 0;
            }
        }
    }
}
