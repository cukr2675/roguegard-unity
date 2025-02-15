using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public abstract class MpSkillIntrinsicOptionScript : ScriptIntrinsicOption.Script
    {
        /// <summary>
        /// <see cref="parent"/> を型ごとに別の変数とするためジェネリック型にしている
        /// </summary>
        public abstract class MpSkillSortedIntrinsic<T> : ReferableScript, ISkill, ISortedIntrinsic
            where T : MpSkillSortedIntrinsic<T>
        {
            private static ScriptIntrinsicOption parent;

            public string Name => parent.DescriptionName;
            public virtual Sprite Icon => parent.Icon;
            public virtual Color Color => parent.Color;
            public virtual string Caption => parent.Caption;
            public virtual IRogueDetails Details => parent.Details;

            public abstract IRogueMethodTarget Target { get; }
            public abstract IRogueMethodRange Range { get; }
            public abstract int RequiredMp { get; }
            public virtual Spanning<IKeyword> AmmoCategories => Spanning<IKeyword>.Empty;

            public int Lv { get; }

            protected MpSkillSortedIntrinsic(ScriptIntrinsicOption parent, int lv)
            {
                if (parent != null) { MpSkillSortedIntrinsic<T>.parent = parent; }
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

                int requiredMp;
                if (RequiredMp >= 1)
                {
                    requiredMp = StatsEffectedValues.GetRequiredMp(self, RequiredMp);
                    if (self.Main.Stats.Mp < requiredMp)
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
                    requiredMp = 0;
                }

                // スキルによって MP を回復することを考慮して、あらかじめ消費しておく
                var stats = self.Main.Stats;
                var beforeMp = stats.Mp;
                stats.SetMp(self, stats.Mp - requiredMp);

                var result = Activate(self, user, activationDepth, arg);
                if (!result)
                {
                    // 失敗したら MP を元に戻す
                    stats.SetMp(self, beforeMp, true);
                }
                return result;
            }
            protected abstract bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);

            public virtual int GetAtk(RogueObj self, out bool additionalEffect)
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

            public virtual bool Equals(MpSkillSortedIntrinsic<T> other)
            {
                return other.GetType() == GetType() && other.Lv == Lv;
            }

            public bool Equals(ISkill obj)
            {
                return obj is MpSkillSortedIntrinsic<T> other && Equals(other);
            }

            public override bool Equals(object obj)
            {
                return obj is MpSkillSortedIntrinsic<T> other && Equals(other);
            }

            public override int GetHashCode()
            {
                return Lv.GetHashCode();
            }
        }
    }
}
