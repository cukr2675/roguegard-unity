using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard
{
    [Objforming.Formable]
    public class LearnedSkillList //: IReadOnlyList<ISkill>
    {
        private readonly List<LearnedSkill> skills = new List<LearnedSkill>();

        public ISkill this[int index] => skills[index].Skill;

        public int Count => skills.Count;

        public void Add(ISkill skill, MainInfoSetType sourceType)
        {
            var contain = Contains(skill, sourceType);
            if (contain) return;

            var item = new LearnedSkill(skill, sourceType);
            skills.Add(item);
        }

        public bool Remove(ISkill skill, MainInfoSetType sourceType)
        {
            var item = new LearnedSkill(skill, sourceType);
            return skills.Remove(item);
        }

        public void Clear()
        {
            skills.Clear();
        }

        public void Clear(MainInfoSetType sourceType)
        {
            for (int i = skills.Count - 1; i >= 0; i--)
            {
                if (skills[i].SourceType == sourceType) { skills.RemoveAt(i); }
            }
        }

        public bool Contains(ISkill skill, MainInfoSetType sourceType)
        {
            var item = new LearnedSkill(skill, sourceType);
            foreach (var listItem in skills)
            {
                var eqSkill = listItem.Skill.Equals(item.Skill);
                var eqType = listItem.SourceType == item.SourceType;
                if (eqSkill && eqType) return true;
            }
            return skills.Contains(item);
        }

        private IEnumerator<ISkill> GetEnumerator() => skills.Select(x => x.Skill).GetEnumerator();

        [Objforming.Formable]
        private struct LearnedSkill : System.IEquatable<LearnedSkill>
        {
            public ISkill Skill { get; }

            public MainInfoSetType SourceType { get; }

            public LearnedSkill(ISkill skill, MainInfoSetType sourceType)
            {
                Skill = skill;
                SourceType = sourceType;
            }

            public bool Equals(LearnedSkill other)
            {
                return other.Skill.Equals(Skill) && other.SourceType == SourceType;
            }

            public override bool Equals(object obj)
            {
                return obj is LearnedSkill other && Equals(other);
            }

            public override int GetHashCode()
            {
                return Skill.GetHashCode() ^ SourceType.GetHashCode();
            }
        }
    }
}
