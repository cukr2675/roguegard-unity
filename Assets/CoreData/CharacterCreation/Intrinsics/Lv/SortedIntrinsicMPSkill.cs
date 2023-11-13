using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public abstract class SortedIntrinsicMPSkill : ReferableScript, ISkill, ISortedIntrinsic
    {
        public abstract string Name { get; }
        public virtual Sprite Icon => null;
        public virtual Color Color => Color.white;
        public virtual string Caption => null;
        public virtual object Details => null;

        public abstract IRogueMethodTarget Target { get; }
        public abstract IRogueMethodRange Range { get; }
        public abstract int RequiredMP { get; }
        public virtual Spanning<IKeyword> AmmoCategories => Spanning<IKeyword>.Empty;

        public int Lv { get; }

        protected SortedIntrinsicMPSkill(int lv)
        {
            Lv = lv;
        }

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (self.Main.Stats.Lv < Lv)
            {
                // レベルダウンしてもスキルは失わないが、発動できなくなる。
                if (RogueDevice.Primary == self)
                {
                    RogueDevice.Add(DeviceKw.AppendText, "レベルが足りない");
                }
                return false;
            }

            int requiredMP;
            if (RequiredMP >= 1)
            {
                requiredMP = StatsEffectedValues.GetRequiredMP(self, RequiredMP);
                if (self.Main.Stats.MP < requiredMP)
                {
                    if (RogueDevice.Primary.Player == self)
                    {
                        RogueDevice.Add(DeviceKw.AppendText, "MPが足りない");
                    }
                    return false;
                }
            }
            else
            {
                requiredMP = 0;
            }

            // スキルによって MP を回復することを考慮して、あらかじめ消費しておく
            var stats = self.Main.Stats;
            var beforeMP = stats.MP;
            stats.SetMP(self, stats.MP - requiredMP);

            var result = Activate(self, user, activationDepth, arg);
            if (!result)
            {
                // 失敗したら MP を元に戻す
                stats.SetMP(self, beforeMP, true);
            }
            return result;
        }
        protected abstract bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);

        public virtual int GetATK(RogueObj self, out bool additionalEffect)
        {
            additionalEffect = false;
            return 0;
        }

        void ISortedIntrinsic.LevelUpToLv(RogueObj self, MainInfoSetType infoSetType)
        {
            self.Main.Skills.Add(this, infoSetType);
            if (RogueDevice.Primary.Player.Main.Stats.Party.Members.Contains(self))
            {
                RogueDevice.Add(DeviceKw.AppendText, this);
                RogueDevice.Add(DeviceKw.AppendText, "を習得した！\n");
            }
        }

        void ISortedIntrinsic.LevelDownFromLv(RogueObj self, MainInfoSetType infoSetType)
        {
            // レベルダウンしてもスキルは失わないが、発動できなくなる。
        }

        void ISortedIntrinsic.Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base)
        {
            // 変化前のスキルは失わないので、変化前に戻るときも追加しない。
            if (polymorph2Base) return;

            if (self.Main.Stats.Lv >= Lv)
            {
                self.Main.Skills.Add(this, infoSetType);
            }
        }

        void ISortedIntrinsic.Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph)
        {
            // 変化前のスキルは失わない。
            if (base2Polymorph) return;

            self.Main.Skills.Remove(this, infoSetType);
        }

        public virtual bool Equals(SortedIntrinsicMPSkill other)
        {
            return other.GetType() == GetType() && other.Lv == Lv;
        }

        public bool Equals(ISkill obj)
        {
            return obj is SortedIntrinsicMPSkill other && Equals(other);
        }

        public override bool Equals(object obj)
        {
            return obj is SortedIntrinsicMPSkill other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Lv.GetHashCode();
        }
    }
}
