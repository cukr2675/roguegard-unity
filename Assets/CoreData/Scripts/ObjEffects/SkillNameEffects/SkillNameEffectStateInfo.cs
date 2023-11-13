using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class SkillNameEffectStateInfo
    {
        public static void AddFromInfoSet(RogueObj self, ISkillNameEffect skillNameEffect)
        {
            if (!self.TryGet<Info>(out var info))
            {
                self.SetInfo(new Info());
            }
            info.AddFromInfoSet(self, skillNameEffect);
        }

        public static void AddFromRogueEffect(RogueObj self, ISkillNameEffect skillNameEffect)
        {
            if (!self.TryGet<Info>(out var info))
            {
                self.SetInfo(new Info());
            }
            info.AddFromRogueEffect(self, skillNameEffect);
        }

        public static void Remove(RogueObj self, ISkillNameEffect skillNameEffect)
        {
            if (!self.TryGet<Info>(out var info))
            {
                info = new Info();
                self.SetInfo(info);
            }
            info.Remove(skillNameEffect);
        }

        public static void GetEffectedName(RogueNameBuilder refName, RogueObj self, ISkill skill)
        {
            self.Main.TryOpenRogueEffects(self);
            if (!self.TryGet<Info>(out var info))
            {
                refName.Clear();
                refName.Append(skill.Name);
                return;
            }
            info.GetEffectedName(refName, skill);
        }

        [ObjectFormer.IgnoreRequireRelationalComponent]
        private class Info : IRogueObjInfo
        {
            private readonly List<ISkillNameEffect> _skillNameEffects = new List<ISkillNameEffect>();

            public Spanning<ISkillNameEffect> SkillNameEffects => _skillNameEffects;

            bool IRogueObjInfo.IsExclusedWhenSerialize => true;

            public void AddFromInfoSet(RogueObj self, ISkillNameEffect skillNameEffect)
            {
                if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningEffects) throw new RogueException();
                if (skillNameEffect == null) throw new System.ArgumentNullException(nameof(skillNameEffect));

                for (int i = 0; i < _skillNameEffects.Count; i++)
                {
                    // 同じ Order の要素が存在するときその手前に追加する。
                    if (_skillNameEffects[i] != null && _skillNameEffects[i].Order >= skillNameEffect.Order)
                    {
                        _skillNameEffects.Insert(i, skillNameEffect);
                        return;
                    }
                }
                _skillNameEffects.Add(skillNameEffect);
            }

            public void AddFromRogueEffect(RogueObj self, ISkillNameEffect skillNameEffect)
            {
                if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningInfoSet) throw new RogueException();
                if (skillNameEffect == null) throw new System.ArgumentNullException(nameof(skillNameEffect));

                for (int i = _skillNameEffects.Count - 1; i >= 0; i--)
                {
                    // 同じ Order の要素が存在するときその後ろに追加する。
                    if (_skillNameEffects[i] != null && _skillNameEffects[i].Order <= skillNameEffect.Order)
                    {
                        _skillNameEffects.Insert(i + 1, skillNameEffect);
                        return;
                    }
                }
                _skillNameEffects.Insert(0, skillNameEffect);
            }

            public bool Remove(ISkillNameEffect skillNameEffect)
            {
                return _skillNameEffects.Remove(skillNameEffect);
            }

            public void GetEffectedName(RogueNameBuilder refName, ISkill skill)
            {
                refName.Clear();
                refName.Append(skill.Name);
                foreach (var item in _skillNameEffects)
                {
                    item.GetEffectedName(refName, skill);
                }
            }

            bool IRogueObjInfo.CanStack(IRogueObjInfo other) => true;
            IRogueObjInfo IRogueObjInfo.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => new Info();
            IRogueObjInfo IRogueObjInfo.ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
