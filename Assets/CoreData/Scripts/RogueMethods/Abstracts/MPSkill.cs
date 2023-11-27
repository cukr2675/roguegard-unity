using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class MPSkill : ReferableScript, ISkill
    {
        public abstract string Name { get; }
        public virtual Sprite Icon => null;
        public virtual Color Color => Color.white;
        public virtual string Caption => null;
        public virtual IRogueDetails Details => null;

        public abstract IRogueMethodTarget Target { get; }
        public abstract IRogueMethodRange Range { get; }
        public abstract int RequiredMP { get; }
        public virtual Spanning<IKeyword> AmmoCategories => Spanning<IKeyword>.Empty;

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
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

        public virtual bool Equals(ISkill other)
        {
            return other.GetType() == GetType();
        }

        public override bool Equals(object obj)
        {
            return obj is MPSkill other && Equals(other);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
