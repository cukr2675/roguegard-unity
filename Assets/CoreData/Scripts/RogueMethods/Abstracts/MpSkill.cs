using UnityEngine;

namespace Roguegard
{
    public abstract class MpSkill : ReferableScript, ISkill
    {
        public abstract string Name { get; }
        public virtual Sprite Icon => null;
        public virtual Color Color => Color.white;
        public virtual string Caption => null;
        public virtual IRogueDetails Details => null;

        public abstract IRogueMethodTarget Target { get; }
        public abstract IRogueMethodRange Range { get; }
        public abstract int RequiredMp { get; }
        public virtual Spanning<IKeyword> AmmoCategories => Spanning<IKeyword>.Empty;

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
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

        public virtual bool Equals(ISkill other)
        {
            return other.GetType() == GetType();
        }

        public override bool Equals(object obj)
        {
            return obj is MpSkill other && Equals(other);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
