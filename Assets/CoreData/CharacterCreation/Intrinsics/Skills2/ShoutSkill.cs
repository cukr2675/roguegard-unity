using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class ShoutSkill : MpSkillIntrinsicScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOptionAsset parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(parent, lv);
        }

        [Objforming.Formable]
        private class SortedIntrinsic : MpSkillSortedIntrinsic<SortedIntrinsic>
        {
            public override IRogueMethodTarget Target => null;
            public override IRogueMethodRange Range => null;
            public override int RequiredMp => 2;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOptionAsset parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                // 同じフロアにいる敵を隣に引き寄せる
                var random = RogueRandom.Primary;
                var location = self.Location;
                foreach (var obj in location.Space.Objs)
                {
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

            public override int GetAtk(RogueObj self, out bool additionalEffect)
            {
                // 同じフロアにいる敵を隣に引き寄せる
                additionalEffect = true;
                return 0;
            }
        }
    }
}
